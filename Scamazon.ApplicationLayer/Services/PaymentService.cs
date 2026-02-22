using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

/// <summary>
/// Payment Service - Tích hợp SePay (QR chuyển khoản + Webhook xác nhận)
/// 
/// Luồng hoạt động:
/// 1. User checkout → Backend tạo QR URL từ qr.sepay.vn (chứa thông tin bank, amount, nội dung CK)
/// 2. Mobile app hiển thị QR → User quét bằng app ngân hàng và chuyển khoản
/// 3. SePay nhận biến động số dư → gọi Webhook đến backend
/// 4. Backend kiểm tra nội dung CK match order_code → xác nhận thanh toán
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IConfiguration _configuration;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IConfiguration configuration)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Tạo QR Code URL để thanh toán qua SePay (VietQR)
    /// URL format: https://qr.sepay.vn/img?acc={ACCOUNT}&bank={BANK}&amount={AMOUNT}&des={CONTENT}
    /// </summary>
    public async Task<VNPayResponseDto> CreateVNPayUrlAsync(int userId, VNPayRequestDto request, string ipAddress, string baseUrl)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);

        if (payment == null)
        {
            return new VNPayResponseDto
            {
                Success = false,
                Message = "Không tìm thấy thông tin thanh toán"
            };
        }

        if (payment.Order.UserId != userId)
        {
            return new VNPayResponseDto
            {
                Success = false,
                Message = "Không có quyền truy cập đơn hàng này"
            };
        }

        if (payment.Status == "success")
        {
            return new VNPayResponseDto
            {
                Success = false,
                Message = "Đơn hàng đã được thanh toán"
            };
        }

        // Đọc config SePay
        var sepaySection = _configuration.GetSection("SePay");
        var bankAccount = sepaySection["BankAccount"] ?? "";
        var bankName = sepaySection["BankName"] ?? "MBBank";
        var accountName = sepaySection["AccountName"] ?? "";

        // Nội dung CK = order_code (để SePay webhook match)
        var transferContent = payment.Order.OrderCode;
        var amount = (long)payment.Amount;

        // Tạo QR URL từ qr.sepay.vn
        var qrUrl = $"https://qr.sepay.vn/img?acc={bankAccount}&bank={bankName}&amount={amount}&des={Uri.EscapeDataString(transferContent)}";

        // Cập nhật transaction ID = order_code
        payment.TransactionId = transferContent;
        await _paymentRepository.UpdatePaymentAsync(payment);

        return new VNPayResponseDto
        {
            Success = true,
            Message = "Tạo mã QR thanh toán thành công",
            Data = new VNPayDataDto
            {
                PaymentUrl = qrUrl
            }
        };
    }

    /// <summary>
    /// Xử lý Webhook từ SePay khi có giao dịch mới
    /// SePay gửi POST JSON:
    /// {
    ///   "id": 123,
    ///   "gateway": "MBBank",
    ///   "transactionDate": "2024-01-01 12:00:00",
    ///   "accountNumber": "0123456789",
    ///   "transferType": "in",
    ///   "transferAmount": 150000,
    ///   "accumulated": 500000,
    ///   "code": null,
    ///   "content": "SCM202401011200001234",
    ///   "referenceCode": "FT123456",
    ///   "description": "..."
    /// }
    /// </summary>
    public async Task<BaseResponseDto> ProcessVNPayCallbackAsync(Dictionary<string, string> webhookData)
    {
        // 1. Lấy nội dung chuyển khoản (chính là order_code)
        var content = webhookData.GetValueOrDefault("content", "");
        var amountStr = webhookData.GetValueOrDefault("transferAmount", "0");
        var transferType = webhookData.GetValueOrDefault("transferType", "");

        // Chỉ xử lý giao dịch tiền vào
        if (transferType != "in")
        {
            return new BaseResponseDto { Success = true, Message = "Ignored: not incoming transfer" };
        }

        if (string.IsNullOrEmpty(content))
        {
            return new BaseResponseDto { Success = false, Message = "Missing transfer content" };
        }

        // 2. Tìm order_code trong nội dung CK
        // Nội dung CK có thể chứa text thêm từ ngân hàng, nên ta tìm order_code pattern
        var payment = await _paymentRepository.GetByTransactionIdAsync(content.Trim());

        // Nếu không tìm thấy chính xác, thử tìm order_code pattern "SCM..." trong content
        if (payment == null)
        {
            // Tìm chuỗi bắt đầu bằng "SCM" trong content
            var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (word.StartsWith("SCM", StringComparison.OrdinalIgnoreCase))
                {
                    payment = await _paymentRepository.GetByTransactionIdAsync(word.Trim());
                    if (payment != null) break;
                }
            }
        }

        if (payment == null)
        {
            return new BaseResponseDto { Success = false, Message = "Không tìm thấy đơn hàng tương ứng" };
        }

        if (payment.Status == "success")
        {
            return new BaseResponseDto { Success = true, Message = "Đơn hàng đã được xác nhận trước đó" };
        }

        // 3. Kiểm tra số tiền
        if (decimal.TryParse(amountStr, out var transferAmount))
        {
            if (transferAmount < payment.Amount)
            {
                return new BaseResponseDto
                {
                    Success = false,
                    Message = $"Số tiền chuyển ({transferAmount:N0}) nhỏ hơn tổng đơn hàng ({payment.Amount:N0})"
                };
            }
        }

        // 4. Xác nhận thanh toán thành công
        payment.Status = "success";
        payment.PaidAt = DateTime.UtcNow;
        payment.PaymentData = JsonSerializer.Serialize(webhookData);
        await _paymentRepository.UpdatePaymentAsync(payment);

        // 5. Cập nhật trạng thái order → confirmed
        await _orderRepository.UpdateOrderStatusAsync(payment.OrderId, "confirmed");

        return new BaseResponseDto { Success = true, Message = "Thanh toán thành công" };
    }

    /// <summary>
    /// Kiểm tra trạng thái thanh toán - Mobile app dùng polling
    /// Gọi định kỳ (mỗi 3-5s) sau khi hiển thị QR để biết khi nào thanh toán xong
    /// </summary>
    public async Task<PaymentStatusResponseDto> CheckPaymentStatusAsync(int userId, int orderId)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(orderId);

        if (payment == null)
        {
            return new PaymentStatusResponseDto
            {
                Success = false,
                Message = "Không tìm thấy thông tin thanh toán"
            };
        }

        // Kiểm tra quyền sở hữu
        if (payment.Order.UserId != userId)
        {
            return new PaymentStatusResponseDto
            {
                Success = false,
                Message = "Không có quyền truy cập đơn hàng này"
            };
        }

        return new PaymentStatusResponseDto
        {
            Success = true,
            Message = payment.Status == "success" ? "Thanh toán thành công" : "Đang chờ thanh toán",
            Data = new PaymentStatusDataDto
            {
                OrderCode = payment.Order.OrderCode,
                PaymentStatus = payment.Status ?? "pending",
                OrderStatus = payment.Order.Status ?? "pending",
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                PaidAt = payment.PaidAt
            }
        };
    }
}
