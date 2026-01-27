using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho User Repository
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Kiểm tra username đã tồn tại chưa
    /// </summary>
    Task<bool> UsernameExistsAsync(string username);

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Thêm user mới vào database
    /// </summary>
    Task<User> CreateUserAsync(User user);

    /// <summary>
    /// Lấy user theo username
    /// </summary>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Lấy user theo id
    /// </summary>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Thêm admin activity log
    /// </summary>
    Task AddAdminActivityLogAsync(AdminActivityLog log);

    /// <summary>
    /// Lưu FCM token cho user
    /// </summary>
    Task SaveDeviceTokenAsync(int userId, string token, string? deviceType);

    /// <summary>
    /// Xóa FCM token
    /// </summary>
    Task RemoveDeviceTokenAsync(int userId, string token);

    /// <summary>
    /// Xóa tất cả FCM token của user
    /// </summary>
    Task RemoveAllDeviceTokensAsync(int userId);
}
