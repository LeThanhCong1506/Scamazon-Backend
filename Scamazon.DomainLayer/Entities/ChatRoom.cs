using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

/// <summary>
/// Phòng chat - Real-time Chat
/// </summary>
public partial class ChatRoom
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? StoreId { get; set; }

    public string? Status { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual Store? Store { get; set; }

    public virtual User User { get; set; } = null!;
}
