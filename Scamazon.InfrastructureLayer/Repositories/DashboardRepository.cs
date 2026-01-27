using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

/// <summary>
/// Repository cho Dashboard
/// </summary>
public class DashboardRepository : IDashboardRepository
{
    private readonly ScamazonDbContext _context;

    public DashboardRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy thống kê dashboard từ view
    /// </summary>
    public async Task<VAdminDashboardStat?> GetDashboardStatsAsync()
    {
        return await _context.VAdminDashboardStats.FirstOrDefaultAsync();
    }
}
