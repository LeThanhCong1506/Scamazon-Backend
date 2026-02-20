using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

/// <summary>
/// Repository cho Admin operations (Dashboard stats, Activity logs)
/// </summary>
public class AdminRepository : IAdminRepository
{
    private readonly ScamazonDbContext _context;

    public AdminRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    public async Task<int> CountCustomersAsync()
    {
        return await _context.Users.CountAsync(u => u.Role == "customer");
    }

    public async Task<int> CountNewCustomers7DaysAsync()
    {
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        return await _context.Users.CountAsync(u =>
            u.Role == "customer" && u.CreatedAt >= sevenDaysAgo);
    }

    public async Task<int> CountActiveProductsAsync()
    {
        return await _context.Products.CountAsync(p => p.IsActive == true);
    }

    public async Task<int> CountLowStockProductsAsync()
    {
        return await _context.Products.CountAsync(p =>
            p.IsActive == true && p.StockQuantity <= 10);
    }

    public async Task<int> CountOrdersByStatusAsync(string status)
    {
        return await _context.Orders.CountAsync(o => o.Status == status);
    }

    public async Task<int> CountTodayOrdersAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Orders.CountAsync(o => o.CreatedAt >= today);
    }

    public async Task<decimal> GetTodayRevenueAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Orders
            .Where(o => o.Status == "delivered" && o.CreatedAt >= today)
            .SumAsync(o => o.Total);
    }

    public async Task<decimal> GetWeekRevenueAsync()
    {
        var weekAgo = DateTime.UtcNow.AddDays(-7);
        return await _context.Orders
            .Where(o => o.Status == "delivered" && o.CreatedAt >= weekAgo)
            .SumAsync(o => o.Total);
    }

    public async Task<decimal> GetMonthRevenueAsync()
    {
        var monthAgo = DateTime.UtcNow.AddDays(-30);
        return await _context.Orders
            .Where(o => o.Status == "delivered" && o.CreatedAt >= monthAgo)
            .SumAsync(o => o.Total);
    }

    public async Task<int> CountActiveChatRoomsAsync()
    {
        return await _context.ChatRooms.CountAsync(cr => cr.Status == "active");
    }

    public async Task LogActivityAsync(AdminActivityLog log)
    {
        _context.AdminActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
