using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    /// <summary>
    /// Lấy giỏ hàng của user
    /// </summary>
    public async Task<CartResponseDto> GetCartAsync(int userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            return new CartResponseDto
            {
                Success = true,
                Data = new CartDataDto
                {
                    CartId = 0,
                    Items = new List<CartItemDto>(),
                    Subtotal = 0,
                    TotalItems = 0
                }
            };
        }

        var items = cart.CartItems.Select(ci => new CartItemDto
        {
            Id = ci.Id,
            ProductId = ci.ProductId,
            ProductName = ci.Product.Name,
            ProductImage = ci.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true)?.ImageUrl
                ?? ci.Product.ProductImages.FirstOrDefault()?.ImageUrl,
            Price = ci.Product.Price,
            SalePrice = ci.Product.SalePrice,
            Quantity = ci.Quantity,
            ItemTotal = (ci.Product.SalePrice ?? ci.Product.Price) * ci.Quantity,
            StockQuantity = ci.Product.StockQuantity ?? 0,
            IsActive = ci.Product.IsActive ?? true
        }).ToList();

        return new CartResponseDto
        {
            Success = true,
            Data = new CartDataDto
            {
                CartId = cart.Id,
                Items = items,
                Subtotal = items.Sum(i => i.ItemTotal),
                TotalItems = items.Sum(i => i.Quantity)
            }
        };
    }

    /// <summary>
    /// Thêm sản phẩm vào giỏ hàng
    /// </summary>
    public async Task<CartResponseDto> AddToCartAsync(int userId, AddToCartRequestDto request)
    {
        // 1. Lấy hoặc tạo giỏ hàng
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null)
        {
            cart = await _cartRepository.CreateCartAsync(userId);
        }

        // 2. Kiểm tra sản phẩm đã có trong giỏ chưa
        var existingItem = await _cartRepository.GetCartItemAsync(cart.Id, request.ProductId);

        if (existingItem != null)
        {
            // Cộng thêm số lượng
            existingItem.Quantity += request.Quantity;
            await _cartRepository.UpdateCartItemAsync(existingItem);
        }
        else
        {
            // Thêm item mới
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            await _cartRepository.AddCartItemAsync(newItem);
        }

        // 3. Trả về giỏ hàng mới nhất
        return await GetCartAsync(userId);
    }

    /// <summary>
    /// Cập nhật số lượng item trong giỏ
    /// </summary>
    public async Task<CartResponseDto> UpdateCartItemAsync(int userId, int itemId, UpdateCartItemRequestDto request)
    {
        var item = await _cartRepository.GetCartItemByIdAsync(itemId);

        if (item == null || item.Cart.UserId != userId)
        {
            return new CartResponseDto
            {
                Success = false,
                Message = "Không tìm thấy item trong giỏ hàng"
            };
        }

        item.Quantity = request.Quantity;
        await _cartRepository.UpdateCartItemAsync(item);

        return await GetCartAsync(userId);
    }

    /// <summary>
    /// Xóa item khỏi giỏ hàng
    /// </summary>
    public async Task<BaseResponseDto> RemoveCartItemAsync(int userId, int itemId)
    {
        var item = await _cartRepository.GetCartItemByIdAsync(itemId);

        if (item == null || item.Cart.UserId != userId)
        {
            return new BaseResponseDto
            {
                Success = false,
                Message = "Không tìm thấy item trong giỏ hàng"
            };
        }

        await _cartRepository.RemoveCartItemAsync(itemId);

        return new BaseResponseDto
        {
            Success = true,
            Message = "Đã xóa sản phẩm khỏi giỏ hàng"
        };
    }
}
