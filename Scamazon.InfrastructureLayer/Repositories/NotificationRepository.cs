using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ScamazonDbContext _context;

    public NotificationRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId, int page, int limit)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        await _context.Notifications
            .Where(n => n.Id == notificationId && n.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && n.IsRead == false)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }

    public async Task<List<DeviceToken>> GetActiveDeviceTokensByUserIdAsync(int userId)
    {
        return await _context.DeviceTokens
            .Where(dt => dt.UserId == userId && dt.IsActive == true)
            .ToListAsync();
    }

    public async Task<List<DeviceToken>> GetAllActiveDeviceTokensAsync()
    {
        return await _context.DeviceTokens
            .Where(dt => dt.IsActive == true)
            .ToListAsync();
    }
}
