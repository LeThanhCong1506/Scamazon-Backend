using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

public interface IOrderService
{
    Task<CreateOrderResponseDto> CreateOrderAsync(int userId, CreateOrderRequestDto request);
    Task<OrderListResponseDto> GetMyOrdersAsync(int userId);
    Task<OrderDetailResponseDto> GetOrderDetailAsync(int userId, int orderId);
    Task<AdminOrderListResponseDto> GetAllOrdersAsync(int page, int limit, string? status);
    Task<BaseResponseDto> UpdateOrderStatusAsync(int orderId, string status, int adminId, string? ipAddress);
}
