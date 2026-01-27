using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

/// <summary>
/// Service xử lý Dashboard
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    /// <summary>
    /// Lấy thống kê dashboard cho admin
    /// </summary>
    public async Task<DashboardStatsResponseDto> GetDashboardStatsAsync()
    {
        var stats = await _dashboardRepository.GetDashboardStatsAsync();

        if (stats == null)
        {
            return new DashboardStatsResponseDto
            {
                Success = true,
                Data = new DashboardStatsDataDto()
            };
        }

        return new DashboardStatsResponseDto
        {
            Success = true,
            Data = new DashboardStatsDataDto
            {
                Customers = new CustomerStatsDto
                {
                    Total = stats.TotalCustomers ?? 0,
                    New7days = stats.NewCustomers7days ?? 0
                },
                Products = new ProductStatsDto
                {
                    Total = stats.TotalProducts ?? 0,
                    LowStock = stats.LowStockProducts ?? 0
                },
                Orders = new OrderStatsDto
                {
                    Pending = stats.PendingOrders ?? 0,
                    Confirmed = stats.ConfirmedOrders ?? 0,
                    Shipping = stats.ShippingOrders ?? 0,
                    Delivered = stats.DeliveredOrders ?? 0,
                    Today = stats.OrdersToday ?? 0
                },
                Revenue = new RevenueStatsDto
                {
                    Today = stats.RevenueToday ?? 0,
                    Week = stats.Revenue7days ?? 0,
                    Month = stats.Revenue30days ?? 0
                },
                Chats = new ChatStatsDto
                {
                    Active = stats.ActiveChats ?? 0
                }
            }
        };
    }
}
