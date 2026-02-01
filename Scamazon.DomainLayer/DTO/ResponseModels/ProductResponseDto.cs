namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO response cho danh sách sản phẩm
/// </summary>
public class ProductListResponseDto
{
    public bool Success { get; set; }
    public ProductListDataDto? Data { get; set; }
    public string? Message { get; set; }
    public List<ValidationErrorDto>? Errors { get; set; }
}

public class ProductListDataDto
{
    public List<ProductSummaryDto> Products { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

/// <summary>
/// DTO cho sản phẩm trong danh sách (summary)
/// </summary>
public class ProductSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int? StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public string? PrimaryImage { get; set; }
    public decimal AvgRating { get; set; }
    public long ReviewCount { get; set; }
    public int? SoldCount { get; set; }
    public bool? IsFeatured { get; set; }
}

/// <summary>
/// DTO cho pagination info
/// </summary>
public class PaginationDto
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int Limit { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrev { get; set; }
}

/// <summary>
/// DTO response cho chi tiết sản phẩm
/// </summary>
public class ProductDetailResponseDto
{
    public bool Success { get; set; }
    public ProductDetailDto? Data { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// DTO cho chi tiết sản phẩm
/// </summary>
public class ProductDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? DetailDescription { get; set; }
    public Dictionary<string, string>? Specifications { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int? DiscountPercent { get; set; }
    public int? StockQuantity { get; set; }
    public string StockStatus { get; set; } = null!;
    public CategoryInfoDto? Category { get; set; }
    public BrandInfoDto? Brand { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();
    public RatingSummaryDto RatingSummary { get; set; } = new();
    public int? ViewCount { get; set; }
    public int? SoldCount { get; set; }
    public bool? IsFeatured { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CategoryInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
}

public class BrandInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? LogoUrl { get; set; }
}

public class ProductImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? AltText { get; set; }
    public bool? IsPrimary { get; set; }
    public int? SortOrder { get; set; }
}

public class RatingSummaryDto
{
    public decimal AvgRating { get; set; }
    public int TotalReviews { get; set; }
    public RatingBreakdownDto RatingBreakdown { get; set; } = new();
}

public class RatingBreakdownDto
{
    public int Five { get; set; }
    public int Four { get; set; }
    public int Three { get; set; }
    public int Two { get; set; }
    public int One { get; set; }
}