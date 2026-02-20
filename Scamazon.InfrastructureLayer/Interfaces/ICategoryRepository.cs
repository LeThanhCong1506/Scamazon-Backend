using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Category Repository
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Lấy tất cả categories active
    /// </summary>
    Task<List<Category>> GetAllActiveAsync();

    /// <summary>
    /// Lấy categories theo parent_id
    /// </summary>
    Task<List<Category>> GetByParentIdAsync(int? parentId);

    /// <summary>
    /// Lấy category theo id
    /// </summary>
    Task<Category?> GetByIdAsync(int id);

    /// <summary>
    /// Kiểm tra slug đã tồn tại chưa
    /// </summary>
    Task<bool> SlugExistsAsync(string slug);

    /// <summary>
    /// Tạo category mới
    /// </summary>
    Task<Category> CreateAsync(Category category);

    /// <summary>
    /// Cập nhật category
    /// </summary>
    Task<Category> UpdateAsync(Category category);

    /// <summary>
    /// Xóa category (soft delete)
    /// </summary>
    Task DeleteAsync(int id);
}
