using Microsoft.EntityFrameworkCore;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

public class OrderService : IOrderService
{
    private readonly ScamazonDbContext _context;
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly INotificationService _notificationService;

    public OrderService(
        ScamazonDbContext context,
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IPaymentRepository paymentRepository,
        IAdminRepository adminRepository,
        INotificationService notificationService)
    {
        _context = context;
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _paymentRepository = paymentRepository;
        _adminRepository = adminRepository;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Tạo đơn hàng - sử dụng Database Transaction
    /// Luồng: Cart → Order → Clear Cart
    /// </summary>
    public async Task<CreateOrderResponseDto> CreateOrderAsync(int userId, CreateOrderRequestDto request)
    {
        // Sử dụng Transaction để đảm bảo data consistency
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Lấy giỏ hàng của user
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return new CreateOrderResponseDto
                {
                    Success = false,
                    Message = "Giỏ hàng trống"
                };
            }

            // 2. Kiểm tra stock và tính toán
            var orderItems = new List<OrderItem>();
            decimal subtotal = 0;

            foreach (var cartItem in cart.CartItems)
            {
                var product = cartItem.Product;

                // Kiểm tra sản phẩm còn active
                if (product.IsActive != true)
                {
                    return new CreateOrderResponseDto
                    {
                        Success = false,
                        Message = $"Sản phẩm '{product.Name}' đã ngừng bán"
                    };
                }

                // Kiểm tra stock
                if ((product.StockQuantity ?? 0) < cartItem.Quantity)
                {
                    return new CreateOrderResponseDto
                    {
                        Success = false,
                        Message = $"Sản phẩm '{product.Name}' chỉ còn {product.StockQuantity} sản phẩm"
                    };
                }

                var price = product.SalePrice ?? product.Price;
                var itemSubtotal = price * cartItem.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true)?.ImageUrl
                        ?? product.ProductImages.FirstOrDefault()?.ImageUrl,
                    Price = price,
                    Quantity = cartItem.Quantity,
                    Subtotal = itemSubtotal
                });

                subtotal += itemSubtotal;

                // 3. Trừ stock
                product.StockQuantity = (product.StockQuantity ?? 0) - cartItem.Quantity;
                product.SoldCount = (product.SoldCount ?? 0) + cartItem.Quantity;
            }

            // 4. Tạo order
            var orderCode = GenerateOrderCode();
            var order = new Order
            {
                OrderCode = orderCode,
                UserId = userId,
                ShippingName = request.ShippingName,
                ShippingPhone = request.ShippingPhone,
                ShippingAddress = request.ShippingAddress,
                ShippingCity = request.ShippingCity,
                ShippingDistrict = request.ShippingDistrict,
                ShippingWard = request.ShippingWard,
                Subtotal = subtotal,
                ShippingFee = 0,
                Discount = 0,
                Total = subtotal,
                Status = "pending",
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 5. Gán OrderId cho order items và insert
            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
            }
            _context.OrderItems.AddRange(orderItems);

            // 6. Tạo payment record
            var payment = new Payment
            {
                OrderId = order.Id,
                PaymentMethod = request.PaymentMethod,
                Amount = order.Total,
                Currency = "VND",
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Payments.Add(payment);

            // 7. Xóa cart items
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            // 8. Commit transaction
            await transaction.CommitAsync();

            return new CreateOrderResponseDto
            {
                Success = true,
                Message = "Đặt hàng thành công",
                Data = new CreateOrderDataDto
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    Total = order.Total,
                    PaymentMethod = request.PaymentMethod
                }
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Lịch sử đơn hàng của user
    /// </summary>
    public async Task<OrderListResponseDto> GetMyOrdersAsync(int userId)
    {
        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

        var data = orders.Select(o => new OrderSummaryDto
        {
            Id = o.Id,
            OrderCode = o.OrderCode,
            Total = o.Total,
            Status = o.Status,
            ItemCount = o.OrderItems.Count,
            FirstProductImage = o.OrderItems.FirstOrDefault()?.ProductImage,
            CreatedAt = o.CreatedAt,
            PaymentMethod = o.Payments.FirstOrDefault()?.PaymentMethod
        }).ToList();

        return new OrderListResponseDto
        {
            Success = true,
            Data = data
        };
    }

    /// <summary>
    /// Chi tiết đơn hàng
    /// </summary>
    public async Task<OrderDetailResponseDto> GetOrderDetailAsync(int userId, int orderId)
    {
        var order = await _orderRepository.GetOrderDetailAsync(orderId);

        if (order == null || order.UserId != userId)
        {
            return new OrderDetailResponseDto
            {
                Success = false,
                Message = "Không tìm thấy đơn hàng"
            };
        }

        var payment = order.Payments.FirstOrDefault();

        return new OrderDetailResponseDto
        {
            Success = true,
            Data = new OrderDetailDataDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                ShippingName = order.ShippingName,
                ShippingPhone = order.ShippingPhone,
                ShippingAddress = order.ShippingAddress,
                ShippingCity = order.ShippingCity,
                ShippingDistrict = order.ShippingDistrict,
                ShippingWard = order.ShippingWard,
                Subtotal = order.Subtotal,
                ShippingFee = order.ShippingFee ?? 0,
                Discount = order.Discount ?? 0,
                Total = order.Total,
                Status = order.Status,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductImage = oi.ProductImage,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    Subtotal = oi.Subtotal
                }).ToList(),
                Payment = payment != null ? new PaymentInfoDto
                {
                    PaymentMethod = payment.PaymentMethod,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    TransactionId = payment.TransactionId,
                    PaidAt = payment.PaidAt
                } : null
            }
        };
    }

    /// <summary>
    /// [Admin] Lấy danh sách toàn bộ đơn hàng
    /// </summary>
    public async Task<AdminOrderListResponseDto> GetAllOrdersAsync(int page, int limit, string? status)
    {
        var (orders, totalCount) = await _orderRepository.GetAllOrdersAsync(page, limit, status);

        var data = orders.Select(o => new AdminOrderSummaryDto
        {
            Id = o.Id,
            OrderCode = o.OrderCode,
            CustomerName = o.User?.FullName ?? o.ShippingName,
            CustomerPhone = o.ShippingPhone,
            Total = o.Total,
            Status = o.Status,
            PaymentMethod = o.Payments.FirstOrDefault()?.PaymentMethod,
            PaymentStatus = o.Payments.FirstOrDefault()?.Status,
            ItemCount = o.OrderItems.Count,
            CreatedAt = o.CreatedAt
        }).ToList();

        return new AdminOrderListResponseDto
        {
            Success = true,
            Data = new AdminOrderListDataDto
            {
                Orders = data,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            }
        };
    }

    /// <summary>
    /// [Admin] Chi tiết đơn hàng - không check userId ownership
    /// </summary>
    public async Task<OrderDetailResponseDto> GetAdminOrderDetailAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderDetailAsync(orderId);

        if (order == null)
        {
            return new OrderDetailResponseDto
            {
                Success = false,
                Message = "Không tìm thấy đơn hàng"
            };
        }

        var payment = order.Payments.FirstOrDefault();

        return new OrderDetailResponseDto
        {
            Success = true,
            Data = new OrderDetailDataDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                ShippingName = order.ShippingName,
                ShippingPhone = order.ShippingPhone,
                ShippingAddress = order.ShippingAddress,
                ShippingCity = order.ShippingCity,
                ShippingDistrict = order.ShippingDistrict,
                ShippingWard = order.ShippingWard,
                Subtotal = order.Subtotal,
                ShippingFee = order.ShippingFee ?? 0,
                Discount = order.Discount ?? 0,
                Total = order.Total,
                Status = order.Status,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductImage = oi.ProductImage,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    Subtotal = oi.Subtotal
                }).ToList(),
                Payment = payment != null ? new PaymentInfoDto
                {
                    PaymentMethod = payment.PaymentMethod,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    TransactionId = payment.TransactionId,
                    PaidAt = payment.PaidAt
                } : null
            }
        };
    }

    /// <summary>
    /// [Admin] Cập nhật trạng thái đơn hàng
    /// </summary>
    public async Task<BaseResponseDto> UpdateOrderStatusAsync(int orderId, string status, int adminId, string? ipAddress)
    {
        var validStatuses = new[] { "pending", "confirmed", "shipping", "delivered", "cancelled" };
        if (!validStatuses.Contains(status))
        {
            return new BaseResponseDto
            {
                Success = false,
                Message = $"Trạng thái không hợp lệ. Cho phép: {string.Join(", ", validStatuses)}"
            };
        }

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            return new BaseResponseDto
            {
                Success = false,
                Message = "Không tìm thấy đơn hàng"
            };
        }

        var oldStatus = order.Status;
        await _orderRepository.UpdateOrderStatusAsync(orderId, status);

        // Ghi log admin activity
        await _adminRepository.LogActivityAsync(new AdminActivityLog
        {
            AdminId = adminId,
            Action = "update_order_status",
            EntityType = "order",
            EntityId = orderId,
            OldData = $"{{\"status\": \"{oldStatus}\"}}",
            NewData = $"{{\"status\": \"{status}\"}}",
            IpAddress = ipAddress
        });

        // Send notification to customer
        try
        {
            await _notificationService.SendOrderStatusNotificationAsync(order.UserId, order.OrderCode, status);
        }
        catch
        {
            // Don't fail order update if notification fails
        }

        return new BaseResponseDto
        {
            Success = true,
            Message = $"Đã cập nhật trạng thái đơn hàng thành '{status}'"
        };
    }

    /// <summary>
    /// Generate order code: SCM + timestamp + random
    /// </summary>
    private static string GenerateOrderCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"SCM{timestamp}{random}";
    }
}
