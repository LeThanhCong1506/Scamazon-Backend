using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Admin Repository
/// </summary>
public interface IAdminRepository
{
    /// <summary>
    /// Đếm tổng số customer
    /// </summary>
    Task<int> CountCustomersAsync();

    /// <summary>
    /// Đếm customer mới trong 7 ngày qua
    /// </summary>
    Task<int> CountNewCustomers7DaysAsync();

    /// <summary>
    /// Đếm tổng sản phẩm active
    /// </summary>
    Task<int> CountActiveProductsAsync();

    /// <summary>
    /// Đếm sản phẩm tồn kho thấp (<=10)
    /// </summary>
    Task<int> CountLowStockProductsAsync();

    /// <summary>
    /// Đếm đơn hàng theo trạng thái
    /// </summary>
    Task<int> CountOrdersByStatusAsync(string status);

    /// <summary>
    /// Đếm đơn hàng hôm nay
    /// </summary>
    Task<int> CountTodayOrdersAsync();

    /// <summary>
    /// Tính doanh thu hôm nay
    /// </summary>
    Task<decimal> GetTodayRevenueAsync();

    /// <summary>
    /// Tính doanh thu tuần
    /// </summary>
    Task<decimal> GetWeekRevenueAsync();

    /// <summary>
    /// Tính doanh thu tháng
    /// </summary>
    Task<decimal> GetMonthRevenueAsync();

    /// <summary>
    /// Đếm phòng chat active
    /// </summary>
    Task<int> CountActiveChatRoomsAsync();

    /// <summary>
    /// Ghi log hoạt động admin
    /// </summary>
    Task LogActivityAsync(AdminActivityLog log);
}
