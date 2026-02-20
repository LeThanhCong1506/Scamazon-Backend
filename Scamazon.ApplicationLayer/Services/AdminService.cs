using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.Interfaces;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MV.ApplicationLayer.Services;

/// <summary>
/// Service xử lý Admin operations (Dashboard, CRUD)
/// </summary>
public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;

    public AdminService(
        IAdminRepository adminRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository)
    {
        _adminRepository = adminRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
    }

    // ==================== DASHBOARD ====================

    public async Task<DashboardStatsResponseDto> GetDashboardStatsAsync()
    {
        var totalCustomers = await _adminRepository.CountCustomersAsync();
        var newCustomers = await _adminRepository.CountNewCustomers7DaysAsync();
        var totalProducts = await _adminRepository.CountActiveProductsAsync();
        var lowStock = await _adminRepository.CountLowStockProductsAsync();
        var pending = await _adminRepository.CountOrdersByStatusAsync("pending");
        var confirmed = await _adminRepository.CountOrdersByStatusAsync("confirmed");
        var shipping = await _adminRepository.CountOrdersByStatusAsync("shipping");
        var delivered = await _adminRepository.CountOrdersByStatusAsync("delivered");
        var todayOrders = await _adminRepository.CountTodayOrdersAsync();
        var todayRevenue = await _adminRepository.GetTodayRevenueAsync();
        var weekRevenue = await _adminRepository.GetWeekRevenueAsync();
        var monthRevenue = await _adminRepository.GetMonthRevenueAsync();
        var activeChats = await _adminRepository.CountActiveChatRoomsAsync();

        return new DashboardStatsResponseDto
        {
            Success = true,
            Data = new DashboardStatsDataDto
            {
                Customers = new CustomerStatsDto { Total = totalCustomers, New7Days = newCustomers },
                Products = new ProductStatsDto { Total = totalProducts, LowStock = lowStock },
                Orders = new OrderStatsDto
                {
                    Pending = pending,
                    Confirmed = confirmed,
                    Shipping = shipping,
                    Delivered = delivered,
                    Today = todayOrders
                },
                Revenue = new RevenueStatsDto
                {
                    Today = todayRevenue,
                    Week = weekRevenue,
                    Month = monthRevenue
                },
                Chats = new ChatStatsDto { Active = activeChats }
            }
        };
    }

    // ==================== CATEGORY CRUD ====================

    public async Task<BaseResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request, int adminId, string? ipAddress)
    {
        var slug = GenerateSlug(request.Name);

        // Đảm bảo slug unique
        if (await _categoryRepository.SlugExistsAsync(slug))
        {
            slug = slug + "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
        }

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            ParentId = request.ParentId,
            IsActive = true
        };

        var created = await _categoryRepository.CreateAsync(category);

        // Log activity
        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "create_category",
            EntityType = "category",
            EntityId = created.Id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Tạo danh mục thành công" };
    }

    public async Task<BaseResponseDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto request, int adminId, string? ipAddress)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return new BaseResponseDto { Success = false, Message = "Không tìm thấy danh mục" };
        }

        if (request.Name != null)
        {
            category.Name = request.Name;
            category.Slug = GenerateSlug(request.Name);
            if (await _categoryRepository.SlugExistsAsync(category.Slug))
            {
                category.Slug += "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
            }
        }
        if (request.Description != null) category.Description = request.Description;
        if (request.ImageUrl != null) category.ImageUrl = request.ImageUrl;
        if (request.ParentId != null) category.ParentId = request.ParentId;
        if (request.IsActive != null) category.IsActive = request.IsActive;

        await _categoryRepository.UpdateAsync(category);

        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "update_category",
            EntityType = "category",
            EntityId = id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Cập nhật danh mục thành công" };
    }

    public async Task<BaseResponseDto> DeleteCategoryAsync(int id, int adminId, string? ipAddress)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return new BaseResponseDto { Success = false, Message = "Không tìm thấy danh mục" };
        }

        await _categoryRepository.DeleteAsync(id);

        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "delete_category",
            EntityType = "category",
            EntityId = id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Xóa danh mục thành công" };
    }

    // ==================== BRAND CRUD ====================

    public async Task<BaseResponseDto> CreateBrandAsync(CreateBrandRequestDto request, int adminId, string? ipAddress)
    {
        var slug = GenerateSlug(request.Name);
        if (await _brandRepository.SlugExistsAsync(slug))
        {
            slug += "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
        }

        var brand = new Brand
        {
            Name = request.Name,
            Slug = slug,
            LogoUrl = request.LogoUrl,
            Description = request.Description,
            IsActive = true
        };

        var created = await _brandRepository.CreateAsync(brand);

        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "create_brand",
            EntityType = "brand",
            EntityId = created.Id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Tạo thương hiệu thành công" };
    }

    public async Task<BaseResponseDto> UpdateBrandAsync(int id, UpdateBrandRequestDto request, int adminId, string? ipAddress)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            return new BaseResponseDto { Success = false, Message = "Không tìm thấy thương hiệu" };
        }

        if (request.Name != null)
        {
            brand.Name = request.Name;
            brand.Slug = GenerateSlug(request.Name);
            if (await _brandRepository.SlugExistsAsync(brand.Slug))
            {
                brand.Slug += "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
            }
        }
        if (request.LogoUrl != null) brand.LogoUrl = request.LogoUrl;
        if (request.Description != null) brand.Description = request.Description;
        if (request.IsActive != null) brand.IsActive = request.IsActive;

        await _brandRepository.UpdateAsync(brand);

        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "update_brand",
            EntityType = "brand",
            EntityId = id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Cập nhật thương hiệu thành công" };
    }

    public async Task<BaseResponseDto> DeleteBrandAsync(int id, int adminId, string? ipAddress)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            return new BaseResponseDto { Success = false, Message = "Không tìm thấy thương hiệu" };
        }

        await _brandRepository.DeleteAsync(id);

        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "delete_brand",
            EntityType = "brand",
            EntityId = id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Xóa thương hiệu thành công" };
    }

    // ==================== PRODUCT CRUD ====================

    public async Task<BaseResponseDto> CreateProductAsync(CreateProductRequestDto request, int adminId, string? ipAddress)
    {
        var slug = GenerateSlug(request.Name);
        if (await _productRepository.SlugExistsAsync(slug))
        {
            slug += "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
        }

        var product = new Product
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            DetailDescription = request.DetailDescription,
            Specifications = request.Specifications,
            Price = request.Price,
            SalePrice = request.SalePrice,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            IsFeatured = request.IsFeatured,
            IsActive = true,
            ViewCount = 0,
            SoldCount = 0
        };

        var created = await _productRepository.CreateAsync(product);

        // Thêm ảnh sản phẩm
        if (request.Images != null && request.Images.Count > 0)
        {
            var images = request.Images.Select(img => new ProductImage
            {
                ProductId = created.Id,
                ImageUrl = img.Url,
                IsPrimary = img.IsPrimary,
                SortOrder = img.SortOrder
            }).ToList();

            await _productRepository.AddProductImagesAsync(images);
        }

        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "create_product",
            EntityType = "product",
            EntityId = created.Id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Tạo sản phẩm thành công" };
    }

    public async Task<BaseResponseDto> UpdateProductAsync(int id, UpdateProductRequestDto request, int adminId, string? ipAddress)
    {
        var product = await _productRepository.GetByIdForAdminAsync(id);
        if (product == null)
        {
            return new BaseResponseDto { Success = false, Message = "Không tìm thấy sản phẩm" };
        }

        if (request.Name != null)
        {
            product.Name = request.Name;
            product.Slug = GenerateSlug(request.Name);
            if (await _productRepository.SlugExistsAsync(product.Slug))
            {
                product.Slug += "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
            }
        }
        if (request.Description != null) product.Description = request.Description;
        if (request.DetailDescription != null) product.DetailDescription = request.DetailDescription;
        if (request.Specifications != null) product.Specifications = request.Specifications;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.SalePrice.HasValue) product.SalePrice = request.SalePrice;
        if (request.StockQuantity.HasValue) product.StockQuantity = request.StockQuantity;
        if (request.CategoryId.HasValue) product.CategoryId = request.CategoryId;
        if (request.BrandId.HasValue) product.BrandId = request.BrandId;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive;
        if (request.IsFeatured.HasValue) product.IsFeatured = request.IsFeatured;

        product.UpdatedAt = DateTime.UtcNow;
        await _productRepository.UpdateAsync(product);

        // Cập nhật ảnh nếu có
        if (request.Images != null)
        {
            await _productRepository.DeleteProductImagesAsync(id);
            var images = request.Images.Select(img => new ProductImage
            {
                ProductId = id,
                ImageUrl = img.Url,
                IsPrimary = img.IsPrimary,
                SortOrder = img.SortOrder
            }).ToList();
            await _productRepository.AddProductImagesAsync(images);
        }

        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "update_product",
            EntityType = "product",
            EntityId = id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Cập nhật sản phẩm thành công" };
    }

    public async Task<BaseResponseDto> DeleteProductAsync(int id, int adminId, string? ipAddress)
    {
        var product = await _productRepository.GetByIdForAdminAsync(id);
        if (product == null)
        {
            return new BaseResponseDto { Success = false, Message = "Không tìm thấy sản phẩm" };
        }

        await _productRepository.DeleteAsync(id);

        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "delete_product",
            EntityType = "product",
            EntityId = id,
            IpAddress = ipAddress
        });

        return new BaseResponseDto { Success = true, Message = "Xóa sản phẩm thành công" };
    }

    // ==================== HELPER ====================

    /// <summary>
    /// Generate slug từ tên (VD: "iPhone 15 Pro" -> "iphone-15-pro")
    /// </summary>
    private static string GenerateSlug(string name)
    {
        // Chuyển về lowercase
        var slug = name.ToLowerInvariant();

        // Loại bỏ dấu tiếng Việt
        slug = RemoveDiacritics(slug);

        // Thay khoảng trắng và ký tự đặc biệt bằng dấu '-'
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
