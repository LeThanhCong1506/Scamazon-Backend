using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller upload ảnh lên Cloudinary
/// </summary>
[Route("api/upload")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;

    public UploadController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Upload ảnh sản phẩm lên Cloudinary (Admin only)
    /// </summary>
    /// <param name="file">File ảnh (jpg, jpeg, png, webp, max 5MB)</param>
    /// <returns>URL ảnh trên Cloudinary</returns>
    /// <response code="200">Upload thành công, trả về URL</response>
    /// <response code="400">File không hợp lệ</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền admin</response>
    [HttpPost("image")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "Vui lòng chọn file ảnh" });
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { success = false, message = "Chỉ chấp nhận file ảnh: jpg, jpeg, png, webp" });
        }

        // Validate file size (5MB)
        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new { success = false, message = "File ảnh tối đa 5MB" });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var url = await _cloudinaryService.UploadImageAsync(stream, file.FileName);

            return Ok(new
            {
                success = true,
                message = "Upload ảnh thành công",
                data = new { url }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Upload thất bại: {ex.Message}" });
        }
    }
}
