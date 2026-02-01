using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO cho query parameters khi lấy danh sách sản phẩm
/// </summary>
public class ProductQueryRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Trang phải là số nguyên dương")]
    public int Page { get; set; } = 1;

    [Range(1, 50, ErrorMessage = "Limit phải từ 1 đến 50")]
    public int Limit { get; set; } = 10;

    public int? CategoryId { get; set; }

    public int? BrandId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá tối thiểu phải >= 0")]
    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5")]
    public int? MinRating { get; set; }

    public string? Search { get; set; }

    public string SortBy { get; set; } = "created_at";

    public string SortOrder { get; set; } = "desc";

    public bool? IsFeatured { get; set; }
}