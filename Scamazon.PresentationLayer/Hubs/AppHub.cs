using Microsoft.AspNetCore.SignalR;

namespace MV.PresentationLayer.Hubs;

/// <summary>
/// Hub chung cho các luồng dữ liệu của toàn ứng dụng như Orders, Products.
/// </summary>
public class AppHub : Hub
{
    // Không cần viết hàm cụ thể ở đây nếu chỉ dùng để server bắn data về client
}
