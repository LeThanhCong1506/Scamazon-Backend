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
}
