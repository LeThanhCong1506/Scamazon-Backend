using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

/// <summary>
/// Repository cho ProductReview entity
/// </summary>
public class ReviewRepository : IReviewRepository
{
    private readonly ScamazonDbContext _context;

    public ReviewRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy reviews của sản phẩm với pagination
    /// </summary>
    public async Task<(List<ProductReview> Reviews, int TotalCount)> GetByProductIdAsync(
        int productId, int page, int limit, int? rating)
    {
        var query = _context.ProductReviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId);

        if (rating.HasValue)
        {
            query = query.Where(r => r.Rating == rating.Value);
        }

        var totalCount = await query.CountAsync();

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (reviews, totalCount);
    }

    /// <summary>
    /// Lấy rating summary của sản phẩm
    /// </summary>
    public async Task<(decimal AvgRating, int TotalReviews, int[] RatingBreakdown)> GetRatingSummaryAsync(int productId)
    {
        var reviews = await _context.ProductReviews
            .Where(r => r.ProductId == productId)
            .ToListAsync();

        if (!reviews.Any())
        {
            return (0, 0, new int[5]);
        }

        var avgRating = (decimal)reviews.Average(r => r.Rating);
        var totalReviews = reviews.Count;
        var breakdown = new int[5];

        breakdown[0] = reviews.Count(r => r.Rating == 1);
        breakdown[1] = reviews.Count(r => r.Rating == 2);
        breakdown[2] = reviews.Count(r => r.Rating == 3);
        breakdown[3] = reviews.Count(r => r.Rating == 4);
        breakdown[4] = reviews.Count(r => r.Rating == 5);

        return (Math.Round(avgRating, 1), totalReviews, breakdown);
    }

    /// <summary>
    /// Kiểm tra user đã review sản phẩm chưa
    /// </summary>
    public async Task<bool> HasUserReviewedAsync(int productId, int userId)
    {
        return await _context.ProductReviews
            .AnyAsync(r => r.ProductId == productId && r.UserId == userId);
    }

    /// <summary>
    /// Tạo review mới
    /// </summary>
    public async Task<ProductReview> CreateAsync(ProductReview review)
    {
        review.CreatedAt = DateTime.Now;
        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }
}