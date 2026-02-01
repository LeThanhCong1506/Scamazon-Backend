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
}