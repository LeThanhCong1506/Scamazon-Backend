using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.PresentationLayer.Hubs;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller cho Order - Customer + Admin
/// </summary>
[ApiController]
[Route("api")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IHubContext<AppHub> _hubContext;

    public OrdersController(IOrderService orderService, IHubContext<AppHub> hubContext)
    {
        _orderService = orderService;
        _hubContext = hubContext;
    }

    private int GetUserId() => int.Parse(User.FindFirst("user_id")!.Value);

    // ==================== CUSTOMER ====================

    /// <summary>
    /// Tạo đơn hàng (Checkout)
    /// </summary>
    [HttpPost("orders")]
    [Authorize]
    [ProducesResponseType(typeof(CreateOrderResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _orderService.CreateOrderAsync(GetUserId(), request);
        if (!result.Success) return BadRequest(result);
        
        await _hubContext.Clients.All.SendAsync("OrderUpdated");
        return Ok(result);
    }

    /// <summary>
    /// Lịch sử đơn hàng của user
    /// </summary>
    [HttpGet("orders")]
    [Authorize]
    [ProducesResponseType(typeof(OrderListResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders()
    {
        var result = await _orderService.GetMyOrdersAsync(GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Chi tiết đơn hàng
    /// </summary>
    [HttpGet("orders/{id}")]
    [Authorize]
    [ProducesResponseType(typeof(OrderDetailResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderDetail(int id)
    {
        var result = await _orderService.GetOrderDetailAsync(GetUserId(), id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    // ==================== ADMIN ====================

    /// <summary>
    /// [Admin] Lấy danh sách toàn bộ đơn hàng
    /// </summary>
    [HttpGet("admin/orders")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(AdminOrderListResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] string? status = null)
    {
        var result = await _orderService.GetAllOrdersAsync(page, limit, status);
        return Ok(result);
    }

    /// <summary>
    /// [Admin] Chi tiết đơn hàng (không check userId)
    /// </summary>
    [HttpGet("admin/orders/{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(OrderDetailResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminOrderDetail(int id)
    {
        var result = await _orderService.GetAdminOrderDetailAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// [Admin] Cập nhật trạng thái đơn hàng
    /// </summary>
    [HttpPut("admin/orders/{id}/status")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var adminId = GetUserId();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _orderService.UpdateOrderStatusAsync(id, request.Status, adminId, ipAddress);

        if (!result.Success) return BadRequest(result);

        await _hubContext.Clients.All.SendAsync("OrderUpdated");
        return Ok(result);
    }
}
