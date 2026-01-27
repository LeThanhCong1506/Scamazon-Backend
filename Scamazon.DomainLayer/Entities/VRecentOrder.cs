using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

public partial class VRecentOrder
{
    public int? Id { get; set; }

    public string? OrderCode { get; set; }

    public decimal? Total { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CustomerUsername { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }
}
