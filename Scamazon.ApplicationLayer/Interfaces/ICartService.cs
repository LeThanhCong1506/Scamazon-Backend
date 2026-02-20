using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

public interface ICartService
{
    Task<CartResponseDto> GetCartAsync(int userId);
    Task<CartResponseDto> AddToCartAsync(int userId, AddToCartRequestDto request);
    Task<CartResponseDto> UpdateCartItemAsync(int userId, int itemId, UpdateCartItemRequestDto request);
    Task<BaseResponseDto> RemoveCartItemAsync(int userId, int itemId);
}
