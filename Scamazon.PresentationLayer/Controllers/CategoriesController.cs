using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller xử lý danh mục sản phẩm
/// </summary>
[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Lấy danh sách danh mục sản phẩm
    /// </summary>
    /// <param name="parent_id">ID danh mục cha (optional)</param>
    /// <returns>Danh sách danh mục</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    [HttpGet]
    [ProducesResponseType(typeof(CategoryListResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories([FromQuery] int? parent_id)
    {
        var result = await _categoryService.GetCategoriesAsync(parent_id);
        return Ok(result);
    }
}