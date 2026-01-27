using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

/// <summary>
/// Interface cho Dashboard Service
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Lấy thống kê dashboard cho admin
    /// </summary>
    Task<DashboardStatsResponseDto> GetDashboardStatsAsync();
}
