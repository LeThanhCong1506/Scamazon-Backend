using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller cho Admin operations - Dashboard, Upload
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ICloudinaryService _cloudinaryService;

    public AdminController(IAdminService adminService, ICloudinaryService cloudinaryService)
    {
        _adminService = adminService;
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Lấy thống kê dashboard cho admin
    /// </summary>
    [HttpGet("dashboard/stats")]
    [ProducesResponseType(typeof(DashboardStatsResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _adminService.GetDashboardStatsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Upload ảnh
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(UploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new BaseResponseDto
            {
                Success = false,
                Message = "File không được để trống"
            });
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
        {
            return BadRequest(new BaseResponseDto
            {
                Success = false,
                Message = "Chỉ hỗ trợ file ảnh (JPEG, PNG, GIF, WebP)"
            });
        }

        try
        {
            var fileName = file.FileName;
            using var stream = file.OpenReadStream();
            var url = await _cloudinaryService.UploadImageAsync(stream, fileName);

            return Ok(new UploadResponseDto
            {
                Success = true,
                Message = "Upload ảnh thành công",
                Data = new UploadDataDto
                {
                    Url = url,
                    FileName = fileName
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponseDto 
            { 
                Success = false, 
                Message = $"Upload thất bại: {ex.Message}" 
            });
        }
    }
}
