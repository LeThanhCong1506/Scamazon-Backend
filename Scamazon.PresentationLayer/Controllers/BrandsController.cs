using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller cho Brand - Public GET + Admin CRUD
/// </summary>
[ApiController]
[Route("api/brands")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;
    private readonly IAdminService _adminService;

    public BrandsController(IBrandService brandService, IAdminService adminService)
    {
        _brandService = brandService;
        _adminService = adminService;
    }

    /// <summary>
    /// Lấy danh sách tất cả brands active (Public)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBrands()
    {
        var result = await _brandService.GetBrandsAsync();
        return Ok(result);
    }

    // ==================== ADMIN CRUD ====================

    /// <summary>
    /// Tạo brand mới (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBrand([FromBody] CreateBrandRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _adminService.CreateBrandAsync(request, userId, ipAddress);
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật brand (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBrand(int id, [FromBody] UpdateBrandRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _adminService.UpdateBrandAsync(id, request, userId, ipAddress);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Xóa brand - soft delete (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _adminService.DeleteBrandAsync(id, userId, ipAddress);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}