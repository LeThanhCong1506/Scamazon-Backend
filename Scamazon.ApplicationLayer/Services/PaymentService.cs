using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MV.ApplicationLayer.Interfaces;
using MV.ApplicationLayer.Utils;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

/// <summary>
/// Payment Service - Tích hợp VNPay (redirect-based payment + IPN webhook)
///
/// Luồng hoạt động:
/// 1. User checkout → Backend tạo URL thanh toán VNPay có ký HMAC-SHA512
/// 2. Mobile app mở URL trong WebView → User thanh toán trên trang VNPay
/// 3. VNPay redirect user về Return URL → WebView bắt vnp_ResponseCode
/// 4. VNPay gửi IPN (Instant Payment Notification) đến backend
/// 5. Backend xác thực chữ ký, cập nhật trạng thái thanh toán
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
    /// Tạo URL thanh toán VNPay với chữ ký HMAC-SHA512
    /// URL sẽ được mở trong WebView của mobile app
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

        // Đọc config VNPay
        var vnpaySection = _configuration.GetSection("VNPay");
        var tmnCode = vnpaySection["TmnCode"] ?? "";
        var hashSecret = vnpaySection["HashSecret"] ?? "";
        var paymentUrl = vnpaySection["PaymentUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        var returnUrl = vnpaySection["ReturnUrl"] ?? $"{baseUrl}/api/payments/vnpay-return";

        if (string.IsNullOrWhiteSpace(tmnCode) || string.IsNullOrWhiteSpace(hashSecret))
        {
            return new VNPayResponseDto
            {
                Success = false,
                Message = "Chưa cấu hình VNPay (TmnCode/HashSecret). Vui lòng liên hệ admin."
            };
        }

        // VNPay yêu cầu amount * 100 (không có phần thập phân)
        var vnpAmount = (long)(payment.Amount * 100);
        var vnpTxnRef = payment.Order.OrderCode; // Mã tham chiếu giao dịch = OrderCode
        
        // VNPay yêu cầu timestamp theo giờ Việt Nam (UTC+7)
        // Trên server Render (UTC), DateTime.Now sẽ trả về giờ UTC → VNPay sẽ thấy đã hết hạn
        var vietnamNow = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));
        var vnpCreateDate = vietnamNow.ToString("yyyyMMddHHmmss");
        var vnpExpireDate = vietnamNow.AddMinutes(15).ToString("yyyyMMddHHmmss");

        // Build tham số theo thứ tự alphabet (SortedList tự sort)
        var vnpParams = new SortedList<string, string>
        {
            { "vnp_Version", vnpaySection["Version"] ?? "2.1.0" },
            { "vnp_Command", vnpaySection["Command"] ?? "pay" },
            { "vnp_TmnCode", tmnCode },
            { "vnp_Amount", vnpAmount.ToString() },
            { "vnp_CreateDate", vnpCreateDate },
            { "vnp_CurrCode", vnpaySection["CurrCode"] ?? "VND" },
            { "vnp_IpAddr", string.IsNullOrWhiteSpace(ipAddress) ? "127.0.0.1" : ipAddress },
            { "vnp_Locale", vnpaySection["Locale"] ?? "vn" },
            { "vnp_OrderInfo", $"Thanh toan don hang {vnpTxnRef}" },
            { "vnp_OrderType", vnpaySection["OrderType"] ?? "other" },
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_TxnRef", vnpTxnRef },
            { "vnp_ExpireDate", vnpExpireDate }
        };

        var signedUrl = VnPayHelper.CreatePaymentUrl(vnpParams, paymentUrl, hashSecret);

        // Lưu transaction ID = OrderCode để match khi IPN về
        payment.TransactionId = vnpTxnRef;
        await _paymentRepository.UpdatePaymentAsync(payment);

        return new VNPayResponseDto
        {
            Success = true,
            Message = "Tạo URL thanh toán VNPay thành công",
            Data = new VNPayDataDto
            {
                PaymentUrl = signedUrl
            }
        };
    }

    /// <summary>
    /// Xử lý IPN (Instant Payment Notification) từ VNPay
    /// VNPay gọi endpoint này sau khi giao dịch hoàn tất (GET với query params)
    /// Phải trả về { RspCode: "00", Message: "Confirm Success" } để VNPay ghi nhận
    /// </summary>
    public async Task<BaseResponseDto> ProcessVNPayCallbackAsync(Dictionary<string, string> vnpayData)
    {
        // 1. Xác thực chữ ký HMAC-SHA512
        var hashSecret = _configuration.GetSection("VNPay")["HashSecret"] ?? "";
        if (!VnPayHelper.ValidateSignature(vnpayData, hashSecret))
        {
            return new BaseResponseDto { Success = false, Message = "97" }; // VNPay error code: invalid signature
        }

        // 2. Lấy các thông tin chính
        var responseCode = vnpayData.GetValueOrDefault("vnp_ResponseCode", "");
        var transactionStatus = vnpayData.GetValueOrDefault("vnp_TransactionStatus", "");
        var txnRef = vnpayData.GetValueOrDefault("vnp_TxnRef", "");
        var vnpAmountStr = vnpayData.GetValueOrDefault("vnp_Amount", "0");

        // 3. Tìm payment theo OrderCode (= vnp_TxnRef)
        var payment = await _paymentRepository.GetByTransactionIdAsync(txnRef);
        if (payment == null)
        {
            return new BaseResponseDto { Success = false, Message = "01" }; // VNPay error code: order not found
        }

        // 4. Idempotency: đã xác nhận trước đó
        if (payment.Status == "success")
        {
            return new BaseResponseDto { Success = true, Message = "00" }; // Already confirmed
        }

        // 5. Kiểm tra số tiền (VNPay gửi amount * 100)
        if (long.TryParse(vnpAmountStr, out var vnpAmount))
        {
            var expectedAmount = (long)(payment.Amount * 100);
            if (vnpAmount != expectedAmount)
            {
                return new BaseResponseDto { Success = false, Message = "04" }; // VNPay error code: amount invalid
            }
        }

        // 6. Kiểm tra kết quả giao dịch
        if (responseCode == "00" && transactionStatus == "00")
        {
            // Thanh toán thành công
            payment.Status = "success";
            payment.PaidAt = DateTime.UtcNow;
            payment.PaymentData = JsonSerializer.Serialize(vnpayData);
            await _paymentRepository.UpdatePaymentAsync(payment);

            await _orderRepository.UpdateOrderStatusAsync(payment.OrderId, "confirmed");

            return new BaseResponseDto { Success = true, Message = "Thanh toán thành công" };
        }
        else
        {
            // Thanh toán thất bại hoặc bị hủy
            payment.Status = "failed";
            payment.PaymentData = JsonSerializer.Serialize(vnpayData);
            await _paymentRepository.UpdatePaymentAsync(payment);

            return new BaseResponseDto { Success = false, Message = $"Thanh toán thất bại: {responseCode}" };
        }
    }

    /// <summary>
    /// Xử lý Return URL từ VNPay - user được redirect về sau khi thanh toán
    /// Chỉ xác thực chữ ký và trả kết quả, KHÔNG cập nhật DB (để IPN xử lý)
    /// Trả JSON để WebView của mobile app đọc và điều hướng
    /// </summary>
    public async Task<BaseResponseDto> ProcessVNPayReturnAsync(Dictionary<string, string> vnpayData)
    {
        var hashSecret = _configuration.GetSection("VNPay")["HashSecret"] ?? "";
        if (!VnPayHelper.ValidateSignature(vnpayData, hashSecret))
        {
            return new BaseResponseDto { Success = false, Message = "Chữ ký không hợp lệ" };
        }

        var responseCode = vnpayData.GetValueOrDefault("vnp_ResponseCode", "");
        var txnRef = vnpayData.GetValueOrDefault("vnp_TxnRef", "");

        if (responseCode == "00")
        {
            return new BaseResponseDto { Success = true, Message = "Thanh toán thành công" };
        }

        return new BaseResponseDto { Success = false, Message = $"Thanh toán thất bại hoặc bị hủy (code: {responseCode})" };
    }

    /// <summary>
    /// Kiểm tra trạng thái thanh toán - Mobile app dùng polling mỗi 3 giây
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
