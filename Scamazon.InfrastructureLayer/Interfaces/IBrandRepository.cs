using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Brand Repository
/// </summary>
public interface IBrandRepository
{
    /// <summary>
    /// Lấy tất cả brands active
    /// </summary>
    Task<List<Brand>> GetAllActiveAsync();

    /// <summary>
    /// Lấy brand theo id
    /// </summary>
    Task<Brand?> GetByIdAsync(int id);

    /// <summary>
    /// Kiểm tra slug đã tồn tại chưa
    /// </summary>
    Task<bool> SlugExistsAsync(string slug);

    /// <summary>
    /// Tạo brand mới
    /// </summary>
    Task<Brand> CreateAsync(Brand brand);

    /// <summary>
    /// Cập nhật brand
    /// </summary>
    Task<Brand> UpdateAsync(Brand brand);

    /// <summary>
    /// Xóa brand (soft delete)
    /// </summary>
    Task DeleteAsync(int id);
}
