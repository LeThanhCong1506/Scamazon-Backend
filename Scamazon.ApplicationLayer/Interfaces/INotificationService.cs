using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

public interface INotificationService
{
    Task<NotificationListResponseDto> GetNotificationsAsync(int userId, int page, int limit);
    Task<BaseResponseDto> SendNotificationAsync(SendNotificationRequestDto request);
    Task SendPushNotificationAsync(int userId, string title, string body, Dictionary<string, string>? data = null);
    Task SendOrderStatusNotificationAsync(int userId, string orderCode, string newStatus);
    Task SendChatNotificationAsync(int userId, string senderName, string message, int chatRoomId);
    Task<BaseResponseDto> MarkAsReadAsync(int notificationId, int userId);
    Task<BaseResponseDto> MarkAllAsReadAsync(int userId);
}
