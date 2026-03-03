using System;

namespace MV.DomainLayer.Entities;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Otp { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
}
