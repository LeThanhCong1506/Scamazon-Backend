using System.Text.Json;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

/// <summary>
/// Service xử lý logic cho Product
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public ProductService(
        IProductRepository productRepository,
        IReviewRepository reviewRepository,
        IUserRepository userRepository)
    {
        _productRepository = productRepository;
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Lấy danh sách sản phẩm với filter, sort, pagination
    /// </summary>
    public async Task<ProductListResponseDto> GetProductsAsync(ProductQueryRequestDto query)
    {
        // Validate sort_by
        var validSortFields = new[] { "price", "sold_count", "created_at" };
        if (!string.IsNullOrEmpty(query.SortBy) && !validSortFields.Contains(query.SortBy.ToLower()))
        {
            return new ProductListResponseDto
            {
                Success = false,
                Message = "Tham số không hợp lệ",
                Errors = new List<ValidationErrorDto>
                {
                    new() { Field = "sort_by", Message = "Giá trị sort_by không hợp lệ" }
                }
            };
        }

        // Validate sort_order
        var validSortOrders = new[] { "asc", "desc" };
        if (!string.IsNullOrEmpty(query.SortOrder) && !validSortOrders.Contains(query.SortOrder.ToLower()))
        {
            return new ProductListResponseDto
            {
                Success = false,
                Message = "Tham số không hợp lệ",
                Errors = new List<ValidationErrorDto>
                {
                    new() { Field = "sort_order", Message = "Giá trị sort_order không hợp lệ" }
                }
            };
        }

        // Validate max_price >= min_price
        if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MaxPrice < query.MinPrice)
        {
            return new ProductListResponseDto
            {
                Success = false,
                Message = "Tham số không hợp lệ",
                Errors = new List<ValidationErrorDto>
                {
                    new() { Field = "max_price", Message = "Giá tối đa phải >= giá tối thiểu" }
                }
            };
        }

        var (products, totalCount) = await _productRepository.GetProductsAsync(query);

        // Get primary images for all products
        var productIds = products.Where(p => p.Id.HasValue).Select(p => p.Id!.Value).ToList();
        var primaryImages = await _productRepository.GetPrimaryImagesAsync(productIds);

        // Get category and brand names
        var productSummaries = products.Select(p => new ProductSummaryDto
        {
            Id = p.Id ?? 0,
            Name = p.Name ?? string.Empty,
            Slug = p.Slug ?? string.Empty,
            Description = p.Description,
            Price = p.Price ?? 0,
            SalePrice = p.SalePrice,
            StockQuantity = p.StockQuantity,
            CategoryId = p.CategoryId,
            BrandId = p.BrandId,
            PrimaryImage = p.Id.HasValue && primaryImages.ContainsKey(p.Id.Value) 
                ? primaryImages[p.Id.Value] 
                : null,
            AvgRating = p.AvgRating ?? 0,
            ReviewCount = p.ReviewCount ?? 0,
            SoldCount = p.SoldCount,
            IsFeatured = p.IsFeatured
        }).ToList();

        var totalPages = (int)Math.Ceiling((double)totalCount / query.Limit);

        return new ProductListResponseDto
        {
            Success = true,
            Data = new ProductListDataDto
            {
                Products = productSummaries,
                Pagination = new PaginationDto
                {
                    CurrentPage = query.Page,
                    TotalPages = totalPages,
                    TotalItems = totalCount,
                    Limit = query.Limit,
                    HasNext = query.Page < totalPages,
                    HasPrev = query.Page > 1
                }
            }
        };
    }

    /// <summary>
    /// Lấy chi tiết sản phẩm theo slug
    /// </summary>
    public async Task<ProductDetailResponseDto> GetProductBySlugAsync(string slug)
    {
        var product = await _productRepository.GetBySlugAsync(slug);

        if (product == null)
        {
            return new ProductDetailResponseDto
            {
                Success = false,
                Message = "Không tìm thấy sản phẩm"
            };
        }

        // Tăng view count
        await _productRepository.IncrementViewCountAsync(product.Id);

        // Lấy rating summary
        var (avgRating, totalReviews, ratingBreakdown) = await _reviewRepository.GetRatingSummaryAsync(product.Id);

        // Parse specifications JSON
        Dictionary<string, string>? specs = null;
        if (!string.IsNullOrEmpty(product.Specifications))
        {
            try
            {
                specs = JsonSerializer.Deserialize<Dictionary<string, string>>(product.Specifications);
            }
            catch
            {
                // Ignore JSON parse errors
            }
        }

        // Calculate discount percent
        int? discountPercent = null;
        if (product.SalePrice.HasValue && product.Price > 0)
        {
            discountPercent = (int)Math.Round((1 - (product.SalePrice.Value / product.Price)) * 100);
        }

        // Determine stock status
        var stockStatus = product.StockQuantity switch
        {
            null or 0 => "out_of_stock",
            <= 5 => "low_stock",
            _ => "in_stock"
        };

        return new ProductDetailResponseDto
        {
            Success = true,
            Data = new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                DetailDescription = product.DetailDescription,
                Specifications = specs,
                Price = product.Price,
                SalePrice = product.SalePrice,
                DiscountPercent = discountPercent,
                StockQuantity = product.StockQuantity,
                StockStatus = stockStatus,
                Category = product.Category != null ? new CategoryInfoDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Slug = product.Category.Slug
                } : null,
                Brand = product.Brand != null ? new BrandInfoDto
                {
                    Id = product.Brand.Id,
                    Name = product.Brand.Name,
                    Slug = product.Brand.Slug,
                    LogoUrl = product.Brand.LogoUrl
                } : null,
                Images = product.ProductImages.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    AltText = i.AltText,
                    IsPrimary = i.IsPrimary,
                    SortOrder = i.SortOrder
                }).ToList(),
                RatingSummary = new RatingSummaryDto
                {
                    AvgRating = avgRating,
                    TotalReviews = totalReviews,
                    RatingBreakdown = new RatingBreakdownDto
                    {
                        One = ratingBreakdown[0],
                        Two = ratingBreakdown[1],
                        Three = ratingBreakdown[2],
                        Four = ratingBreakdown[3],
                        Five = ratingBreakdown[4]
                    }
                },
                ViewCount = (product.ViewCount ?? 0) + 1, // Include the increment
                SoldCount = product.SoldCount,
                IsFeatured = product.IsFeatured,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            }
        };
    }

    /// <summary>
    /// Lấy danh sách reviews của sản phẩm
    /// </summary>
    public async Task<ReviewListResponseDto> GetProductReviewsAsync(int productId, int page, int limit, int? rating)
    {
        // Verify product exists
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return new ReviewListResponseDto
            {
                Success = false,
                Message = "Không tìm thấy sản phẩm"
            };
        }

        var (reviews, totalCount) = await _reviewRepository.GetByProductIdAsync(productId, page, limit, rating);

        var totalPages = (int)Math.Ceiling((double)totalCount / limit);

        return new ReviewListResponseDto
        {
            Success = true,
            Data = new ReviewListDataDto
            {
                Reviews = reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    User = new ReviewUserDto
                    {
                        Id = r.User.Id,
                        Username = r.User.Username,
                        FullName = r.User.FullName,
                        AvatarUrl = r.User.AvatarUrl
                    },
                    CreatedAt = r.CreatedAt
                }).ToList(),
                Pagination = new PaginationDto
                {
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalCount,
                    Limit = limit,
                    HasNext = page < totalPages,
                    HasPrev = page > 1
                }
            }
        };
    }

    /// <summary>
    /// Tạo review cho sản phẩm
    /// </summary>
    public async Task<CreateReviewResponseDto> CreateReviewAsync(int productId, int userId, ReviewRequestDto request)
    {
        // Verify product exists
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return new CreateReviewResponseDto
            {
                Success = false,
                Message = "Không tìm thấy sản phẩm"
            };
        }

        // Check if user already reviewed
        var hasReviewed = await _reviewRepository.HasUserReviewedAsync(productId, userId);
        if (hasReviewed)
        {
            return new CreateReviewResponseDto
            {
                Success = false,
                Message = "Bạn đã đánh giá sản phẩm này rồi"
            };
        }

        // Get user info
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new CreateReviewResponseDto
            {
                Success = false,
                Message = "Không tìm thấy thông tin người dùng"
            };
        }

        // Create review
        var review = new ProductReview
        {
            ProductId = productId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        var createdReview = await _reviewRepository.CreateAsync(review);

        return new CreateReviewResponseDto
        {
            Success = true,
            Message = "Đăng đánh giá thành công",
            Data = new ReviewDto
            {
                Id = createdReview.Id,
                Rating = createdReview.Rating,
                Comment = createdReview.Comment,
                User = new ReviewUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl
                },
                CreatedAt = createdReview.CreatedAt
            }
        };
    }
}