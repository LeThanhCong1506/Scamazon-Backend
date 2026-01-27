using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

public partial class ChatMessage
{
    public int Id { get; set; }

    public int ChatRoomId { get; set; }

    public int? SenderId { get; set; }

    public string? MessageType { get; set; }

    public string Content { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public int? ProductId { get; set; }

    public bool? IsFromStore { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public virtual Product? Product { get; set; }

    public virtual User? Sender { get; set; }
}
