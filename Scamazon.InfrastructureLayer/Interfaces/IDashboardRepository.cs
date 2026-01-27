using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Dashboard Repository
/// </summary>
public interface IDashboardRepository
{
    /// <summary>
    /// Lấy thống kê dashboard từ view
    /// </summary>
    Task<VAdminDashboardStat?> GetDashboardStatsAsync();
}
