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
}