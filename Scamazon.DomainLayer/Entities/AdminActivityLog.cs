using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

/// <summary>
/// Ghi log các hoạt động của admin để audit
/// </summary>
public partial class AdminActivityLog
{
    public int Id { get; set; }

    public int AdminId { get; set; }

    public string Action { get; set; } = null!;

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Admin { get; set; } = null!;
}
