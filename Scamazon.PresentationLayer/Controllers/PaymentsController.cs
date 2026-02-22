using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller cho Payment - SePay (QR chuyển khoản)
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
    /// Tạo QR Code thanh toán (SePay VietQR)
    /// Trả về URL ảnh QR để mobile app hiển thị
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
    /// SePay Webhook - nhận thông báo khi có giao dịch mới
    /// SePay gọi POST JSON đến endpoint này
    /// </summary>
    [HttpPost("webhook/sepay")]
    public async Task<IActionResult> SepayWebhook([FromBody] Dictionary<string, object> rawData)
    {
        // Convert object values to strings for processing
        var webhookData = rawData.ToDictionary(
            kv => kv.Key,
            kv => kv.Value?.ToString() ?? "");

        var result = await _paymentService.ProcessVNPayCallbackAsync(webhookData);
        return Ok(result);
    }

    /// <summary>
    /// Kiểm tra trạng thái thanh toán (Mobile app dùng polling)
    /// Mobile app gọi mỗi 3-5 giây sau khi hiển thị QR để biết khi nào thanh toán xong
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
