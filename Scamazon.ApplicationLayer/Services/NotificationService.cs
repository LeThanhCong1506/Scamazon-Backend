using System.Text.Json;
using FirebaseAdmin.Messaging;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.InfrastructureLayer.Interfaces;
using Notification = MV.DomainLayer.Entities.Notification;

namespace MV.ApplicationLayer.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<NotificationListResponseDto> GetNotificationsAsync(int userId, int page, int limit)
    {
        var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId, page, limit);

        var data = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            Title = n.Title,
            Body = n.Body,
            Data = n.Data,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();

        return new NotificationListResponseDto
        {
            Success = true,
            Message = "Lấy danh sách thông báo thành công",
            Data = data
        };
    }

    public async Task<BaseResponseDto> SendNotificationAsync(SendNotificationRequestDto request)
    {
        var notification = new Notification
        {
            UserId = request.UserId,
            Type = request.Type,
            Title = request.Title,
            Body = request.Body,
            Data = request.Data,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.CreateNotificationAsync(notification);

        // Push via FCM
        await SendPushNotificationAsync(request.UserId, request.Title, request.Body ?? "", null);

        return new BaseResponseDto
        {
            Success = true,
            Message = "Gửi thông báo thành công"
        };
    }

    public async Task SendPushNotificationAsync(int userId, string title, string body, Dictionary<string, string>? data = null)
    {
        try
        {
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null) return;

            var deviceTokens = await _notificationRepository.GetActiveDeviceTokensByUserIdAsync(userId);
            if (!deviceTokens.Any()) return;

            var messages = deviceTokens.Select(dt => new Message
            {
                Token = dt.Token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            }).ToList();

            await FirebaseMessaging.DefaultInstance.SendEachAsync(messages);
        }
        catch
        {
            // Fail silently - FCM push is best-effort
        }
    }

    public async Task SendOrderStatusNotificationAsync(int userId, string orderCode, string newStatus)
    {
        var statusMessages = new Dictionary<string, string>
        {
            { "confirmed", "Đơn hàng đã được xác nhận" },
            { "shipping", "Đơn hàng đang được giao" },
            { "delivered", "Đơn hàng đã giao thành công" },
            { "cancelled", "Đơn hàng đã bị hủy" }
        };

        var statusMessage = statusMessages.GetValueOrDefault(newStatus, $"Trạng thái đơn hàng: {newStatus}");
        var title = $"Cập nhật đơn hàng {orderCode}";

        var dataJson = JsonSerializer.Serialize(new { orderCode, status = newStatus });

        var notification = new Notification
        {
            UserId = userId,
            Type = "order_status",
            Title = title,
            Body = statusMessage,
            Data = dataJson,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.CreateNotificationAsync(notification);
        await SendPushNotificationAsync(userId, title, statusMessage, new Dictionary<string, string>
        {
            { "type", "order_status" },
            { "orderCode", orderCode },
            { "status", newStatus }
        });
    }

    public async Task SendChatNotificationAsync(int userId, string senderName, string message, int chatRoomId)
    {
        var title = $"Tin nhắn mới từ {senderName}";
        var body = message.Length > 100 ? message[..100] + "..." : message;

        var dataJson = JsonSerializer.Serialize(new { chatRoomId, senderName });

        var notification = new Notification
        {
            UserId = userId,
            Type = "chat",
            Title = title,
            Body = body,
            Data = dataJson,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.CreateNotificationAsync(notification);
        await SendPushNotificationAsync(userId, title, body, new Dictionary<string, string>
        {
            { "type", "chat" },
            { "chatRoomId", chatRoomId.ToString() }
        });
    }

    public async Task<BaseResponseDto> MarkAsReadAsync(int notificationId, int userId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId, userId);
        return new BaseResponseDto
        {
            Success = true,
            Message = "Đã đánh dấu đã đọc"
        };
    }

    public async Task<BaseResponseDto> MarkAllAsReadAsync(int userId)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId);
        return new BaseResponseDto
        {
            Success = true,
            Message = "Đã đánh dấu tất cả đã đọc"
        };
    }
}
