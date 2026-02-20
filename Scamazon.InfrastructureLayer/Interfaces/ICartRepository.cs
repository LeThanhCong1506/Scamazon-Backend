using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

/// <summary>
/// Interface cho Cart Repository
/// </summary>
public interface ICartRepository
{
    /// <summary>
    /// Lấy giỏ hàng của user (bao gồm CartItems + Product info)
    /// </summary>
    Task<Cart?> GetCartByUserIdAsync(int userId);

    /// <summary>
    /// Tạo giỏ hàng mới cho user
    /// </summary>
    Task<Cart> CreateCartAsync(int userId);

    /// <summary>
    /// Lấy cart item theo cartId và productId
    /// </summary>
    Task<CartItem?> GetCartItemAsync(int cartId, int productId);

    /// <summary>
    /// Lấy cart item theo id
    /// </summary>
    Task<CartItem?> GetCartItemByIdAsync(int itemId);

    /// <summary>
    /// Thêm item vào giỏ
    /// </summary>
    Task<CartItem> AddCartItemAsync(CartItem item);

    /// <summary>
    /// Cập nhật cart item
    /// </summary>
    Task UpdateCartItemAsync(CartItem item);

    /// <summary>
    /// Xóa cart item
    /// </summary>
    Task RemoveCartItemAsync(int itemId);

    /// <summary>
    /// Xóa tất cả items trong giỏ
    /// </summary>
    Task ClearCartAsync(int cartId);
}
