using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Review Repository
/// </summary>
public interface IReviewRepository
{
    /// <summary>
    /// Lấy reviews của sản phẩm với pagination
    /// </summary>
    Task<(List<ProductReview> Reviews, int TotalCount)> GetByProductIdAsync(int productId, int page, int limit, int? rating);

    /// <summary>
    /// Lấy rating summary của sản phẩm
    /// </summary>
    Task<(decimal AvgRating, int TotalReviews, int[] RatingBreakdown)> GetRatingSummaryAsync(int productId);

    /// <summary>
    /// Kiểm tra user đã review sản phẩm chưa
    /// </summary>
    Task<bool> HasUserReviewedAsync(int productId, int userId);

    /// <summary>
    /// Tạo review mới
    /// </summary>
    Task<ProductReview> CreateAsync(ProductReview review);
}