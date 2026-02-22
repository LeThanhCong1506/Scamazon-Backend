using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.PresentationLayer.Hubs;

namespace MV.PresentationLayer.Controllers;

[Route("api")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ICloudinaryService _cloudinaryService;

    public ChatController(IChatService chatService, INotificationService notificationService, IHubContext<ChatHub> hubContext, ICloudinaryService cloudinaryService)
    {
        _chatService = chatService;
        _notificationService = notificationService;
        _hubContext = hubContext;
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Upload ảnh cho chat (bất kỳ user đã đăng nhập)
    /// </summary>
    [HttpPost("chat/upload")]
    [Authorize]
    public async Task<IActionResult> UploadChatImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new BaseResponseDto { Success = false, Message = "File không được để trống" });

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new BaseResponseDto { Success = false, Message = "Chỉ hỗ trợ file ảnh (JPEG, PNG, GIF, WebP)" });

        try
        {
            using var stream = file.OpenReadStream();
            var url = await _cloudinaryService.UploadImageAsync(stream, file.FileName);
            return Ok(new UploadResponseDto
            {
                Success = true,
                Message = "Upload thành công",
                Data = new UploadDataDto { Url = url, FileName = file.FileName }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponseDto { Success = false, Message = $"Upload thất bại: {ex.Message}" });
        }
    }

    [HttpPost("chat/start")]
    [Authorize]
    public async Task<IActionResult> StartChat([FromBody] StartChatRequestDto request)
    {
        var userId = GetUserIdFromToken();
        if (userId == 0) return Unauthorized(new { success = false, message = "Token không hợp lệ" });

        var result = await _chatService.StartChatAsync(userId, request.StoreId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
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

        var isAdmin = User.IsInRole("admin");
        var result = await _chatService.GetMessagesAsync(id, userId, page, limit, isAdmin);
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

            // Send push notification if recipient is offline (Bug 3 fix)
            try
            {
                var roomOwnerId = await _chatService.GetChatRoomOwnerIdAsync(id);
                if (roomOwnerId == userId)
                {
                    // Customer sent message → notify all admins
                    var adminIds = await _chatService.GetAdminUserIdsAsync();
                    foreach (var adminId in adminIds)
                    {
                        if (!ChatHub.IsUserOnline(adminId))
                        {
                            await _notificationService.SendChatNotificationAsync(
                                adminId,
                                result.Data.SenderName ?? "Người dùng",
                                result.Data.Content,
                                id);
                        }
                    }
                }
                else if (roomOwnerId > 0 && !ChatHub.IsUserOnline(roomOwnerId))
                {
                    // Admin sent message → notify customer
                    await _notificationService.SendChatNotificationAsync(
                        roomOwnerId,
                        result.Data.SenderName ?? "Cửa hàng",
                        result.Data.Content,
                        id);
                }
            }
            catch
            {
                // Don't fail message send if notification fails
            }
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
