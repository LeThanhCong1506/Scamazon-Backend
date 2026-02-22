using System;

namespace MV.DomainLayer.Entities;

/// <summary>
/// Danh sách yêu thích - Wishlist
/// </summary>
public partial class Favorite
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
