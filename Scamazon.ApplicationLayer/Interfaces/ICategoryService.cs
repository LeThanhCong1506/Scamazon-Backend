using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

/// <summary>
/// Interface cho Category Service
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Lấy danh sách categories (tree structure)
    /// </summary>
    Task<CategoryListResponseDto> GetCategoriesAsync(int? parentId);
}