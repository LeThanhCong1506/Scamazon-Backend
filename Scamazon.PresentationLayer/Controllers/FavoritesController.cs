using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller cho Danh sách yêu thích - Customer only
/// </summary>
[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    private int GetUserId() => int.Parse(User.FindFirst("user_id")!.Value);

    /// <summary>
    /// Lấy danh sách sản phẩm yêu thích
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(FavoriteListResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavorites()
    {
        var result = await _favoriteService.GetFavoritesAsync(GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách product ID yêu thích (để hiện trái tim đỏ)
    /// </summary>
    [HttpGet("ids")]
    [ProducesResponseType(typeof(FavoriteIdsResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavoriteIds()
    {
        var result = await _favoriteService.GetFavoriteIdsAsync(GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Toggle yêu thích sản phẩm (thêm nếu chưa có, xóa nếu đã có)
    /// </summary>
    [HttpPost("toggle/{productId}")]
    [ProducesResponseType(typeof(FavoriteToggleResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleFavorite(int productId)
    {
        var result = await _favoriteService.ToggleFavoriteAsync(GetUserId(), productId);
        return Ok(result);
    }
}
