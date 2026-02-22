using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

public interface IFavoriteService
{
    Task<FavoriteListResponseDto> GetFavoritesAsync(int userId);
    Task<FavoriteIdsResponseDto> GetFavoriteIdsAsync(int userId);
    Task<FavoriteToggleResponseDto> ToggleFavoriteAsync(int userId, int productId);
}
