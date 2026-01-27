namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO cho response dashboard stats
/// </summary>
public class DashboardStatsResponseDto
{
    public bool Success { get; set; }
    public DashboardStatsDataDto? Data { get; set; }
}

/// <summary>
/// Data thống kê dashboard
/// </summary>
public class DashboardStatsDataDto
{
    public CustomerStatsDto Customers { get; set; } = new();
    public ProductStatsDto Products { get; set; } = new();
    public OrderStatsDto Orders { get; set; } = new();
    public RevenueStatsDto Revenue { get; set; } = new();
    public ChatStatsDto Chats { get; set; } = new();
}

/// <summary>
/// Thống kê khách hàng
/// </summary>
public class CustomerStatsDto
{
    public long Total { get; set; }
    public long New7days { get; set; }
}

/// <summary>
/// Thống kê sản phẩm
/// </summary>
public class ProductStatsDto
{
    public long Total { get; set; }
    public long LowStock { get; set; }
}

/// <summary>
/// Thống kê đơn hàng
/// </summary>
public class OrderStatsDto
{
    public long Pending { get; set; }
    public long Confirmed { get; set; }
    public long Shipping { get; set; }
    public long Delivered { get; set; }
    public long Today { get; set; }
}

/// <summary>
/// Thống kê doanh thu
/// </summary>
public class RevenueStatsDto
{
    public decimal Today { get; set; }
    public decimal Week { get; set; }
    public decimal Month { get; set; }
}

/// <summary>
/// Thống kê chat
/// </summary>
public class ChatStatsDto
{
    public long Active { get; set; }
}
