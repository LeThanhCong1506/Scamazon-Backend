using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

/// <summary>
/// Đơn hàng - Billing &amp; Order Confirmation
/// </summary>
public partial class Order
{
    public int Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public int UserId { get; set; }

    public string ShippingName { get; set; } = null!;

    public string ShippingPhone { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public string? ShippingCity { get; set; }

    public string? ShippingDistrict { get; set; }

    public string? ShippingWard { get; set; }

    public decimal Subtotal { get; set; }

    public decimal? ShippingFee { get; set; }

    public decimal? Discount { get; set; }

    public decimal Total { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
