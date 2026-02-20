namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO cho response Admin Dashboard Stats
/// </summary>
public class DashboardStatsResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public DashboardStatsDataDto? Data { get; set; }
}

public class DashboardStatsDataDto
{
    public CustomerStatsDto Customers { get; set; } = new();
    public ProductStatsDto Products { get; set; } = new();
    public OrderStatsDto Orders { get; set; } = new();
    public RevenueStatsDto Revenue { get; set; } = new();
    public ChatStatsDto Chats { get; set; } = new();
}

public class CustomerStatsDto
{
    public int Total { get; set; }
    public int New7Days { get; set; }
}

public class ProductStatsDto
{
    public int Total { get; set; }
    public int LowStock { get; set; }
}

public class OrderStatsDto
{
    public int Pending { get; set; }
    public int Confirmed { get; set; }
    public int Shipping { get; set; }
    public int Delivered { get; set; }
    public int Today { get; set; }
}

public class RevenueStatsDto
{
    public decimal Today { get; set; }
    public decimal Week { get; set; }
    public decimal Month { get; set; }
}

public class ChatStatsDto
{
    public int Active { get; set; }
}

/// <summary>
/// DTO cho response Upload ảnh
/// </summary>
public class UploadResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public UploadDataDto? Data { get; set; }
}

public class UploadDataDto
{
    public string Url { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
