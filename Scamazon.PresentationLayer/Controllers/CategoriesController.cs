using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller cho Category - Public GET + Admin CRUD
/// </summary>
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IAdminService _adminService;

    public CategoriesController(ICategoryService categoryService, IAdminService adminService)
    {
        _categoryService = categoryService;
        _adminService = adminService;
    }

    /// <summary>
    /// Lấy danh sách tất cả categories active (Public)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] int? parentId)
    {
        var result = await _categoryService.GetCategoriesAsync(parentId);
        return Ok(result);
    }

    // ==================== ADMIN CRUD ====================

    /// <summary>
    /// Tạo category mới (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _adminService.CreateCategoryAsync(request, userId, ipAddress);
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật category (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _adminService.UpdateCategoryAsync(id, request, userId, ipAddress);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Xóa category - soft delete (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _adminService.DeleteCategoryAsync(id, userId, ipAddress);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}