using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

/// <summary>
/// Interface cho Product Service
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Lấy danh sách sản phẩm với filter, sort, pagination
    /// </summary>
    Task<ProductListResponseDto> GetProductsAsync(ProductQueryRequestDto query);

    /// <summary>
    /// Lấy chi tiết sản phẩm theo slug
    /// </summary>
    Task<ProductDetailResponseDto> GetProductBySlugAsync(string slug);

    /// <summary>
    /// Lấy danh sách reviews của sản phẩm
    /// </summary>
    Task<ReviewListResponseDto> GetProductReviewsAsync(int productId, int page, int limit, int? rating);

    /// <summary>
    /// Tạo review cho sản phẩm
    /// </summary>
    Task<CreateReviewResponseDto> CreateReviewAsync(int productId, int userId, ReviewRequestDto request);
}