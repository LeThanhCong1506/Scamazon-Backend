using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller cho Payment - VNPay (redirect-based payment)
/// </summary>
[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Tạo URL thanh toán VNPay có ký HMAC-SHA512
    /// Mobile app nhận URL này và mở trong WebView
    /// </summary>
    [HttpPost("create-qr")]
    [Authorize]
    [ProducesResponseType(typeof(VNPayResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePaymentQR([FromBody] VNPayRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var result = await _paymentService.CreateVNPayUrlAsync(userId, request, ipAddress, baseUrl);

        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// VNPay IPN (Instant Payment Notification)
    /// VNPay server gọi endpoint này (GET với query params) sau khi giao dịch hoàn tất
    /// Phải trả về { RspCode: "00", Message: "Confirm Success" } để VNPay ghi nhận thành công
    /// </summary>
    [HttpGet("vnpay-ipn")]
    public async Task<IActionResult> VnPayIpn()
    {
        var vnpayData = Request.Query.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.ToString());

        var result = await _paymentService.ProcessVNPayCallbackAsync(vnpayData);

        // VNPay yêu cầu response format cụ thể
        var rspCode = result.Success ? "00" : (result.Message?.Length == 2 ? result.Message : "99");
        return Ok(new { RspCode = rspCode, Message = result.Success ? "Confirm Success" : result.Message });
    }

    /// <summary>
    /// VNPay Return URL - User được redirect về đây sau khi thanh toán trên trang VNPay
    /// WebView của mobile app sẽ bắt URL này (detect vnp_ResponseCode trong query string)
    /// Trả JSON để WebView đọc kết quả
    /// </summary>
    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnPayReturn()
    {
        var vnpayData = Request.Query.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.ToString());

        var result = await _paymentService.ProcessVNPayReturnAsync(vnpayData);
        return Ok(result);
    }

    /// <summary>
    /// Kiểm tra trạng thái thanh toán (Mobile app dùng polling mỗi 3 giây)
    /// Dùng song song với WebView để bắt kết quả từ IPN
    /// </summary>
    [HttpGet("status/{orderId}")]
    [Authorize]
    [ProducesResponseType(typeof(PaymentStatusResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckPaymentStatus(int orderId)
    {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var result = await _paymentService.CheckPaymentStatusAsync(userId, orderId);

        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
}
