using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Order Repository
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Tạo đơn hàng mới
    /// </summary>
    Task<Order> CreateOrderAsync(Order order);

    /// <summary>
    /// Lấy đơn hàng theo id
    /// </summary>
    Task<Order?> GetByIdAsync(int id);

    /// <summary>
    /// Lấy đơn hàng chi tiết (kèm items + payment)
    /// </summary>
    Task<Order?> GetOrderDetailAsync(int id);

    /// <summary>
    /// Lấy danh sách đơn hàng của user
    /// </summary>
    Task<List<Order>> GetOrdersByUserIdAsync(int userId);

    /// <summary>
    /// Lấy danh sách toàn bộ đơn hàng (Admin) với phân trang
    /// </summary>
    Task<(List<Order> Orders, int TotalCount)> GetAllOrdersAsync(int page, int limit, string? status);

    /// <summary>
    /// Cập nhật trạng thái đơn hàng
    /// </summary>
    Task UpdateOrderStatusAsync(int orderId, string status);

    /// <summary>
    /// Thêm order items
    /// </summary>
    Task AddOrderItemsAsync(List<OrderItem> items);
}
