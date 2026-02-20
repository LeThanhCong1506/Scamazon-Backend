using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;

namespace MV.PresentationLayer.Controllers;

[Route("api")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("notifications")]
    [Authorize]
    public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = GetUserIdFromToken();
        if (userId == 0) return Unauthorized(new { success = false, message = "Token không hợp lệ" });

        var result = await _notificationService.GetNotificationsAsync(userId, page, limit);
        return Ok(result);
    }

    [HttpPut("notifications/{id}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = GetUserIdFromToken();
        if (userId == 0) return Unauthorized(new { success = false, message = "Token không hợp lệ" });

        var result = await _notificationService.MarkAsReadAsync(id, userId);
        return Ok(result);
    }

    [HttpPut("notifications/read-all")]
    [Authorize]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserIdFromToken();
        if (userId == 0) return Unauthorized(new { success = false, message = "Token không hợp lệ" });

        var result = await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(result);
    }

    [HttpPost("admin/notifications")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

        var result = await _notificationService.SendNotificationAsync(request);
        return Ok(result);
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst("user_id");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }
}
