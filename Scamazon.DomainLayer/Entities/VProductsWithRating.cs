using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

public partial class VProductsWithRating
{
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Description { get; set; }

    public string? DetailDescription { get; set; }

    public string? Specifications { get; set; }

    public decimal? Price { get; set; }

    public decimal? SalePrice { get; set; }

    public int? StockQuantity { get; set; }

    public int? CategoryId { get; set; }

    public int? BrandId { get; set; }

    public int? ViewCount { get; set; }

    public int? SoldCount { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsFeatured { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? AvgRating { get; set; }

    public long? ReviewCount { get; set; }
}
