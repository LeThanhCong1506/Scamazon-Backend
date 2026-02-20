using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId, int page, int limit);
    Task<Notification> CreateNotificationAsync(Notification notification);
    Task MarkAsReadAsync(int notificationId, int userId);
    Task MarkAllAsReadAsync(int userId);
    Task<List<DeviceToken>> GetActiveDeviceTokensByUserIdAsync(int userId);
    Task<List<DeviceToken>> GetAllActiveDeviceTokensAsync();
}
