using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

/// <summary>
/// Bảng quản lý thông tin người dùng - Authentication
/// </summary>
public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Phân quyền: customer = khách hàng, admin = quản trị viên
    /// </summary>
    public string? Role { get; set; }

    public virtual ICollection<AdminActivityLog> AdminActivityLogs { get; set; } = new List<AdminActivityLog>();

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<ChatMessage> ChatMessageAdmins { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatMessage> ChatMessageSenders { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();

    public virtual ICollection<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
}
