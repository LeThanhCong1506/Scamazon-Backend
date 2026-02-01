using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

/// <summary>
/// Service xử lý logic cho Category
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Lấy danh sách categories (tree structure)
    /// </summary>
    public async Task<CategoryListResponseDto> GetCategoriesAsync(int? parentId)
    {
        List<Category> categories;

        if (parentId.HasValue)
        {
            // Lấy categories theo parent_id
            categories = await _categoryRepository.GetByParentIdAsync(parentId);
            return new CategoryListResponseDto
            {
                Success = true,
                Data = categories.Select(MapToCategoryDto).ToList()
            };
        }

        // Lấy tất cả và build tree structure
        categories = await _categoryRepository.GetAllActiveAsync();
        var categoryTree = BuildCategoryTree(categories);

        return new CategoryListResponseDto
        {
            Success = true,
            Data = categoryTree
        };
    }

    /// <summary>
    /// Build category tree từ flat list
    /// </summary>
    private List<CategoryDto> BuildCategoryTree(List<Category> categories)
    {
        var lookup = categories.ToLookup(c => c.ParentId);
        return BuildTree(null, lookup);
    }

    private List<CategoryDto> BuildTree(int? parentId, ILookup<int?, Category> lookup)
    {
        return lookup[parentId]
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                ParentId = c.ParentId,
                Children = BuildTree(c.Id, lookup)
            })
            .ToList();
    }

    private CategoryDto MapToCategoryDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            ParentId = category.ParentId,
            Children = new List<CategoryDto>()
        };
    }
}