using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

/// <summary>
/// Repository cho User entity
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ScamazonDbContext _context;

    public UserRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Kiểm tra username đã tồn tại chưa
    /// </summary>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Thêm user mới vào database
    /// </summary>
    public async Task<User> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;
        user.IsActive = true;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Lấy user theo username
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    /// <summary>
    /// Lấy user theo id
    /// </summary>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    /// <summary>
    /// Thêm admin activity log
    /// </summary>
    public async Task AddAdminActivityLogAsync(AdminActivityLog log)
    {
        log.CreatedAt = DateTime.Now;
        _context.AdminActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Lưu FCM token cho user
    /// </summary>
    public async Task SaveDeviceTokenAsync(int userId, string token, string? deviceType)
    {
        var existingToken = await _context.DeviceTokens
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Token == token);

        if (existingToken != null)
        {
            existingToken.IsActive = true;
            existingToken.UpdatedAt = DateTime.Now;
            existingToken.DeviceType = deviceType;
        }
        else
        {
            var deviceToken = new DeviceToken
            {
                UserId = userId,
                Token = token,
                DeviceType = deviceType,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.DeviceTokens.Add(deviceToken);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Xóa FCM token
    /// </summary>
    public async Task RemoveDeviceTokenAsync(int userId, string token)
    {
        var deviceToken = await _context.DeviceTokens
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Token == token);

        if (deviceToken != null)
        {
            deviceToken.IsActive = false;
            deviceToken.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Xóa tất cả FCM token của user
    /// </summary>
    public async Task RemoveAllDeviceTokensAsync(int userId)
    {
        var tokens = await _context.DeviceTokens
            .Where(d => d.UserId == userId && d.IsActive == true)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsActive = false;
            token.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Cập nhật thông tin user
    /// </summary>
    public async Task<User> UpdateUserAsync(User user)
    {
        user.UpdatedAt = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa (trừ user hiện tại)
    /// </summary>
    public async Task<bool> EmailExistsExceptUserAsync(string email, int userId)
    {
        return await _context.Users
            .AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower() && u.Id != userId);
    }
}
