using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO cho tạo mới Category (Admin)
/// </summary>
public class CreateCategoryRequestDto
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [MaxLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }
}

/// <summary>
/// DTO cho cập nhật Category (Admin)
/// </summary>
public class UpdateCategoryRequestDto
{
    [MaxLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
    public string? Name { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO cho tạo mới Brand (Admin)
/// </summary>
public class CreateBrandRequestDto
{
    [Required(ErrorMessage = "Tên thương hiệu là bắt buộc")]
    [MaxLength(100, ErrorMessage = "Tên thương hiệu tối đa 100 ký tự")]
    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO cho cập nhật Brand (Admin)
/// </summary>
public class UpdateBrandRequestDto
{
    [MaxLength(100, ErrorMessage = "Tên thương hiệu tối đa 100 ký tự")]
    public string? Name { get; set; }

    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO cho tạo mới Product (Admin)
/// </summary>
public class CreateProductRequestDto
{
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [MaxLength(255, ErrorMessage = "Tên sản phẩm tối đa 255 ký tự")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public string? DetailDescription { get; set; }
    public string? Specifications { get; set; }

    [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal Price { get; set; }

    public decimal? SalePrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
    public int StockQuantity { get; set; } = 0;

    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public bool IsFeatured { get; set; } = false;

    public List<ProductImageRequestDto>? Images { get; set; }
}

/// <summary>
/// DTO cho cập nhật Product (Admin)
/// </summary>
public class UpdateProductRequestDto
{
    [MaxLength(255, ErrorMessage = "Tên sản phẩm tối đa 255 ký tự")]
    public string? Name { get; set; }

    public string? Description { get; set; }
    public string? DetailDescription { get; set; }
    public string? Specifications { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal? Price { get; set; }

    public decimal? SalePrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
    public int? StockQuantity { get; set; }

    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsFeatured { get; set; }

    public List<ProductImageRequestDto>? Images { get; set; }
}

/// <summary>
/// DTO cho ảnh sản phẩm khi tạo/cập nhật
/// </summary>
public class ProductImageRequestDto
{
    [Required(ErrorMessage = "URL ảnh là bắt buộc")]
    public string Url { get; set; } = null!;

    public bool IsPrimary { get; set; } = false;
    public int SortOrder { get; set; } = 0;
}
