using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Product Repository
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Lấy danh sách sản phẩm với filter, sort, pagination
    /// </summary>
    Task<(List<VProductsWithRating> Products, int TotalCount)> GetProductsAsync(ProductQueryRequestDto query);

    /// <summary>
    /// Lấy chi tiết sản phẩm theo slug
    /// </summary>
    Task<Product?> GetBySlugAsync(string slug);

    /// <summary>
    /// Lấy sản phẩm theo id
    /// </summary>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>
    /// Tăng view count của sản phẩm
    /// </summary>
    Task IncrementViewCountAsync(int productId);

    /// <summary>
    /// Lấy tất cả images của sản phẩm
    /// </summary>
    Task<List<ProductImage>> GetProductImagesAsync(int productId);

    /// <summary>
    /// Lấy primary image của sản phẩm
    /// </summary>
    Task<string?> GetPrimaryImageAsync(int productId);

    /// <summary>
    /// Lấy primary images của nhiều sản phẩm
    /// </summary>
    Task<Dictionary<int, string?>> GetPrimaryImagesAsync(List<int> productIds);

    /// <summary>
    /// Kiểm tra slug đã tồn tại chưa
    /// </summary>
    Task<bool> SlugExistsAsync(string slug);

    /// <summary>
    /// Tạo sản phẩm mới
    /// </summary>
    Task<Product> CreateAsync(Product product);

    /// <summary>
    /// Cập nhật sản phẩm
    /// </summary>
    Task<Product> UpdateAsync(Product product);

    /// <summary>
    /// Xóa sản phẩm (soft delete)
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Xóa tất cả ảnh của sản phẩm
    /// </summary>
    Task DeleteProductImagesAsync(int productId);

    /// <summary>
    /// Thêm ảnh cho sản phẩm
    /// </summary>
    Task AddProductImagesAsync(List<ProductImage> images);

    /// <summary>
    /// Lấy sản phẩm theo id (bao gồm cả inactive - dành cho Admin)
    /// </summary>
    Task<Product?> GetByIdForAdminAsync(int id);
}
