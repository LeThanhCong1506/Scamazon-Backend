using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

/// <summary>
/// Repository cho Product entity
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ScamazonDbContext _context;

    public ProductRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách sản phẩm với filter, sort, pagination
    /// </summary>
    public async Task<(List<VProductsWithRating> Products, int TotalCount)> GetProductsAsync(ProductQueryRequestDto query)
    {
        var queryable = _context.VProductsWithRatings
            .Where(p => p.IsActive == true);

        // Filter by category
        if (query.CategoryId.HasValue)
        {
            queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        // Filter by brand
        if (query.BrandId.HasValue)
        {
            queryable = queryable.Where(p => p.BrandId == query.BrandId.Value);
        }

        // Filter by price range
        if (query.MinPrice.HasValue)
        {
            queryable = queryable.Where(p => (p.SalePrice ?? p.Price) >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            queryable = queryable.Where(p => (p.SalePrice ?? p.Price) <= query.MaxPrice.Value);
        }

        // Filter by rating
        if (query.MinRating.HasValue)
        {
            queryable = queryable.Where(p => p.AvgRating >= query.MinRating.Value);
        }

        // Filter by featured
        if (query.IsFeatured.HasValue)
        {
            queryable = queryable.Where(p => p.IsFeatured == query.IsFeatured.Value);
        }

        // Search by name
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchTerm = query.Search.ToLower();
            queryable = queryable.Where(p => p.Name != null && p.Name.ToLower().Contains(searchTerm));
        }

        // Get total count
        var totalCount = await queryable.CountAsync();

        // Apply sorting
        queryable = query.SortBy?.ToLower() switch
        {
            "price" => query.SortOrder?.ToLower() == "asc"
                ? queryable.OrderBy(p => p.SalePrice ?? p.Price)
                : queryable.OrderByDescending(p => p.SalePrice ?? p.Price),
            "sold_count" => query.SortOrder?.ToLower() == "asc"
                ? queryable.OrderBy(p => p.SoldCount)
                : queryable.OrderByDescending(p => p.SoldCount),
            _ => query.SortOrder?.ToLower() == "asc"
                ? queryable.OrderBy(p => p.CreatedAt)
                : queryable.OrderByDescending(p => p.CreatedAt)
        };

        // Apply pagination
        var products = await queryable
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .ToListAsync();

        return (products, totalCount);
    }

    /// <summary>
    /// Lấy chi tiết sản phẩm theo slug
    /// </summary>
    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductImages.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive == true);
    }

    /// <summary>
    /// Lấy sản phẩm theo id
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive == true);
    }

    /// <summary>
    /// Tăng view count của sản phẩm
    /// </summary>
    public async Task IncrementViewCountAsync(int productId)
    {
        await _context.Products
            .Where(p => p.Id == productId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => (p.ViewCount ?? 0) + 1));
    }

    /// <summary>
    /// Lấy tất cả images của sản phẩm
    /// </summary>
    public async Task<List<ProductImage>> GetProductImagesAsync(int productId)
    {
        return await _context.ProductImages
            .Where(i => i.ProductId == productId)
            .OrderBy(i => i.SortOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy primary image của sản phẩm
    /// </summary>
    public async Task<string?> GetPrimaryImageAsync(int productId)
    {
        return await _context.ProductImages
            .Where(i => i.ProductId == productId && i.IsPrimary == true)
            .Select(i => i.ImageUrl)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Lấy primary images của nhiều sản phẩm
    /// </summary>
    public async Task<Dictionary<int, string?>> GetPrimaryImagesAsync(List<int> productIds)
    {
        var images = await _context.ProductImages
            .Where(i => productIds.Contains(i.ProductId) && i.IsPrimary == true)
            .Select(i => new { i.ProductId, i.ImageUrl })
            .ToListAsync();

        return images.ToDictionary(i => i.ProductId, i => (string?)i.ImageUrl);
    }
}