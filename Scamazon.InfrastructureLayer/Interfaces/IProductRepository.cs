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
}