using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO thêm sản phẩm vào giỏ hàng
/// </summary>
public class AddToCartRequestDto
{
    [Required(ErrorMessage = "ProductId là bắt buộc")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    public int Quantity { get; set; } = 1;
}

/// <summary>
/// DTO cập nhật số lượng item trong giỏ
/// </summary>
public class UpdateCartItemRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    public int Quantity { get; set; }
}

/// <summary>
/// DTO tạo đơn hàng (Checkout)
/// </summary>
public class CreateOrderRequestDto
{
    [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
    [MaxLength(100)]
    public string ShippingName { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [MaxLength(20)]
    public string ShippingPhone { get; set; } = null!;

    [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
    public string ShippingAddress { get; set; } = null!;

    public string? ShippingCity { get; set; }
    public string? ShippingDistrict { get; set; }
    public string? ShippingWard { get; set; }

    [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
    public string PaymentMethod { get; set; } = "cod"; // vnpay, zalopay, cod

    public string? Note { get; set; }
}

/// <summary>
/// DTO tạo URL thanh toán VNPay
/// </summary>
public class VNPayRequestDto
{
    [Required(ErrorMessage = "OrderId là bắt buộc")]
    public int OrderId { get; set; }
}

/// <summary>
/// DTO cập nhật trạng thái đơn hàng (Admin)
/// </summary>
public class UpdateOrderStatusRequestDto
{
    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public string Status { get; set; } = null!; // pending, confirmed, shipping, delivered, cancelled
}
