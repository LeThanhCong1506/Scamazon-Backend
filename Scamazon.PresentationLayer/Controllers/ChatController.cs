using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.PresentationLayer.Hubs;

namespace MV.PresentationLayer.Controllers;

[Route("api")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatController(IChatService chatService, INotificationService notificationService, IHubContext<ChatHub> hubContext)
    {
        _chatService = chatService;
        _notificationService = notificationService;
        _hubContext = hubContext;
    }

    [HttpGet("chat/conversations")]
    [Authorize]
    public async Task<IActionResult> GetConversations()
    {
        var userId = GetUserIdFromToken();
        if (userId == 0) return Unauthorized(new { success = false, message = "Token không hợp lệ" });

        var result = await _chatService.GetConversationsAsync(userId);
        return Ok(result);
    }

    [HttpGet("chat/{id}/messages")]
    [Authorize]
    public async Task<IActionResult> GetMessages(int id, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = GetUserIdFromToken();
        if (userId == 0) return Unauthorized(new { success = false, message = "Token không hợp lệ" });

        var result = await _chatService.GetMessagesAsync(id, userId, page, limit);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("chat/{id}/messages")]
    [Authorize]
    public async Task<IActionResult> SendMessage(int id, [FromBody] SendMessageRequestDto request)
    {
        var userId = GetUserIdFromToken();
        if (userId == 0) return Unauthorized(new { success = false, message = "Token không hợp lệ" });

        var isFromStore = User.IsInRole("admin");
        var result = await _chatService.SendMessageAsync(id, userId, request, isFromStore);
        if (!result.Success) return BadRequest(result);

        // Broadcast via SignalR
        if (result.Data != null)
        {
            await _hubContext.Clients.Group($"chat_{id}").SendAsync("ReceiveMessage", result.Data);
        }

        return Ok(result);
    }

    [HttpGet("admin/chat")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllChatRooms()
    {
        var result = await _chatService.GetAllChatRoomsAsync();
        return Ok(result);
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst("user_id");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }
}
