namespace MV.DomainLayer.DTO.ResponseModels;

// ==================== CART ====================

/// <summary>
/// Response giỏ hàng
/// </summary>
public class CartResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public CartDataDto? Data { get; set; }
}

public class CartDataDto
{
    public int CartId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public int TotalItems { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? ProductImage { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; }
    public decimal ItemTotal { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
}

// ==================== ORDER ====================

/// <summary>
/// Response tạo đơn hàng
/// </summary>
public class CreateOrderResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public CreateOrderDataDto? Data { get; set; }
}

public class CreateOrderDataDto
{
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = null!;
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string? PaymentUrl { get; set; }
}

/// <summary>
/// Response danh sách đơn hàng
/// </summary>
public class OrderListResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<OrderSummaryDto>? Data { get; set; }
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public decimal Total { get; set; }
    public string? Status { get; set; }
    public int ItemCount { get; set; }
    public string? FirstProductImage { get; set; }
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Response chi tiết đơn hàng
/// </summary>
public class OrderDetailResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public OrderDetailDataDto? Data { get; set; }
}

public class OrderDetailDataDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public string ShippingName { get; set; } = null!;
    public string ShippingPhone { get; set; } = null!;
    public string ShippingAddress { get; set; } = null!;
    public string? ShippingCity { get; set; }
    public string? ShippingDistrict { get; set; }
    public string? ShippingWard { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? Status { get; set; }
    public string? Note { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public PaymentInfoDto? Payment { get; set; }
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? ProductImage { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}

public class PaymentInfoDto
{
    public string PaymentMethod { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
}

// ==================== ADMIN ORDER LIST ====================

/// <summary>
/// Response danh sách đơn hàng cho admin (kèm thông tin user)
/// </summary>
public class AdminOrderListResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public AdminOrderListDataDto? Data { get; set; }
}

public class AdminOrderListDataDto
{
    public List<AdminOrderSummaryDto> Orders { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
}

public class AdminOrderSummaryDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public decimal Total { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    public int ItemCount { get; set; }
    public DateTime? CreatedAt { get; set; }
}

// ==================== VNPAY ====================

/// <summary>
/// Response URL thanh toán VNPay
/// </summary>
public class VNPayResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public VNPayDataDto? Data { get; set; }
}

public class VNPayDataDto
{
    public string PaymentUrl { get; set; } = null!;
}

// ==================== PAYMENT STATUS ====================

/// <summary>
/// Response kiểm tra trạng thái thanh toán (mobile polling)
/// </summary>
public class PaymentStatusResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public PaymentStatusDataDto? Data { get; set; }
}

public class PaymentStatusDataDto
{
    public string OrderCode { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!; // pending, success, failed
    public string OrderStatus { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime? PaidAt { get; set; }
}
