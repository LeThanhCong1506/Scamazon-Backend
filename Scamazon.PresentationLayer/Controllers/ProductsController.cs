using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller xử lý sản phẩm và đánh giá
/// </summary>
[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Lấy User ID từ JWT token
    /// </summary>
    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst("user_id");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    /// <summary>
    /// Lấy danh sách sản phẩm với filter, sort, search, pagination
    /// </summary>
    /// <param name="query">Query parameters</param>
    /// <returns>Danh sách sản phẩm với pagination</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="400">Tham số không hợp lệ</response>
    [HttpGet]
    [ProducesResponseType(typeof(ProductListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductListResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts([FromQuery] ProductQueryRequestDto query)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => new ValidationErrorDto
                {
                    Field = x.Key.ToLower(),
                    Message = e.ErrorMessage
                }))
                .ToList();

            return BadRequest(new ProductListResponseDto
            {
                Success = false,
                Message = "Tham số không hợp lệ",
                Errors = errors
            });
        }

        var result = await _productService.GetProductsAsync(query);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết sản phẩm theo slug
    /// </summary>
    /// <param name="slug">Slug của sản phẩm</param>
    /// <returns>Chi tiết sản phẩm</returns>
    /// <response code="200">Lấy chi tiết thành công</response>
    /// <response code="404">Không tìm thấy sản phẩm</response>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ProductDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductDetailResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductBySlug(string slug)
    {
        var result = await _productService.GetProductBySlugAsync(slug);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách reviews của sản phẩm
    /// </summary>
    /// <param name="id">ID sản phẩm</param>
    /// <param name="page">Trang hiện tại</param>
    /// <param name="limit">Số review mỗi trang</param>
    /// <param name="rating">Filter theo số sao (1-5)</param>
    /// <returns>Danh sách reviews với pagination</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="404">Không tìm thấy sản phẩm</response>
    [HttpGet("{id:int}/reviews")]
    [ProducesResponseType(typeof(ReviewListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReviewListResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductReviews(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] int? rating = null)
    {
        var result = await _productService.GetProductReviewsAsync(id, page, limit, rating);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Đăng review cho sản phẩm
    /// </summary>
    /// <param name="id">ID sản phẩm</param>
    /// <param name="request">Nội dung review</param>
    /// <returns>Review đã tạo</returns>
    /// <response code="201">Tạo review thành công</response>
    /// <response code="400">Đã review hoặc validation error</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="404">Không tìm thấy sản phẩm</response>
    [HttpPost("{id:int}/reviews")]
    [Authorize]
    [ProducesResponseType(typeof(CreateReviewResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateReviewResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CreateReviewResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateReview(int id, [FromBody] ReviewRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => new ValidationErrorDto
                {
                    Field = x.Key.ToLower(),
                    Message = e.ErrorMessage
                }))
                .ToList();

            return BadRequest(new CreateReviewResponseDto
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var userId = GetUserIdFromToken();
        var result = await _productService.CreateReviewAsync(id, userId, request);

        if (!result.Success)
        {
            if (result.Message == "Không tìm thấy sản phẩm")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return StatusCode(StatusCodes.Status201Created, result);
    }
}