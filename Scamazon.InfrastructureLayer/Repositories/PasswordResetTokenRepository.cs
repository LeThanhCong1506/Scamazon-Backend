using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly ScamazonDbContext _context;

    public PasswordResetTokenRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    public async Task SaveOtpAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetValidOtpAsync(string email, string otp)
    {
        return await _context.PasswordResetTokens
            .Where(t => t.Email == email
                     && t.Otp == otp
                     && !t.IsUsed
                     && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task InvalidateOldOtpsAsync(string email)
    {
        var oldTokens = await _context.PasswordResetTokens
            .Where(t => t.Email == email && !t.IsUsed)
            .ToListAsync();

        foreach (var token in oldTokens)
        {
            token.IsUsed = true;
        }

        if (oldTokens.Count > 0)
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAsUsedAsync(PasswordResetToken token)
    {
        token.IsUsed = true;
        await _context.SaveChangesAsync();
    }
}
