using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller cho Giỏ hàng - Customer only
/// </summary>
[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private int GetUserId() => int.Parse(User.FindFirst("user_id")!.Value);

    /// <summary>
    /// Lấy giỏ hàng hiện tại
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cartService.GetCartAsync(GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Thêm sản phẩm vào giỏ hàng
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _cartService.AddToCartAsync(GetUserId(), request);
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật số lượng item trong giỏ
    /// </summary>
    [HttpPut("items/{id}")]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartItemRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _cartService.UpdateCartItemAsync(GetUserId(), id, request);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Xóa item khỏi giỏ hàng
    /// </summary>
    [HttpDelete("items/{id}")]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveCartItem(int id)
    {
        var result = await _cartService.RemoveCartItemAsync(GetUserId(), id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
}
