using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;

    public FavoriteService(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<FavoriteListResponseDto> GetFavoritesAsync(int userId)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(userId);

        var items = favorites.Select(f =>
        {
            var primaryImage = f.Product.ProductImages
                .FirstOrDefault(i => i.IsPrimary == true)?.ImageUrl
                ?? f.Product.ProductImages.FirstOrDefault()?.ImageUrl;

            return new FavoriteItemDto
            {
                Id = f.Id,
                ProductId = f.ProductId,
                ProductName = f.Product.Name,
                ProductImage = primaryImage,
                Price = f.Product.Price,
                SalePrice = f.Product.SalePrice,
                CreatedAt = f.CreatedAt
            };
        }).ToList();

        return new FavoriteListResponseDto
        {
            Success = true,
            Message = "Lấy danh sách yêu thích thành công",
            Data = items
        };
    }

    public async Task<FavoriteIdsResponseDto> GetFavoriteIdsAsync(int userId)
    {
        var ids = await _favoriteRepository.GetFavoriteProductIdsAsync(userId);

        return new FavoriteIdsResponseDto
        {
            Success = true,
            Message = "Lấy danh sách ID yêu thích thành công",
            Data = ids
        };
    }

    public async Task<FavoriteToggleResponseDto> ToggleFavoriteAsync(int userId, int productId)
    {
        var existing = await _favoriteRepository.FindAsync(userId, productId);

        if (existing != null)
        {
            // Remove from favorites
            await _favoriteRepository.DeleteAsync(existing);
            return new FavoriteToggleResponseDto
            {
                Success = true,
                Message = "Đã bỏ yêu thích",
                Data = new FavoriteToggleDataDto
                {
                    IsFavorited = false,
                    ProductId = productId
                }
            };
        }
        else
        {
            // Add to favorites
            var favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };
            await _favoriteRepository.AddAsync(favorite);
            return new FavoriteToggleResponseDto
            {
                Success = true,
                Message = "Đã thêm vào yêu thích",
                Data = new FavoriteToggleDataDto
                {
                    IsFavorited = true,
                    ProductId = productId
                }
            };
        }
    }
}
