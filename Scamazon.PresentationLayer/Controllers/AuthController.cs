using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.PresentationLayer.Middlewares;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller xử lý Authentication (Register, Login, Logout)
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
    /// Lấy IP Address từ request
    /// </summary>
    private string? GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    /// <param name="request">Thông tin đăng ký</param>
    /// <returns>User info và JWT token</returns>
    /// <response code="201">Đăng ký thành công</response>
    /// <response code="400">Validation error hoặc username/email đã tồn tại</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
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

            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ",
                Errors = errors
            });
        }

        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Đăng nhập (Customer và Admin dùng chung)
    /// </summary>
    /// <param name="request">Thông tin đăng nhập</param>
    /// <returns>User info và JWT token</returns>
    /// <response code="200">Đăng nhập thành công</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Username hoặc password không đúng</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
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

            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ",
                Errors = errors
            });
        }

        var ipAddress = GetIpAddress();
        var result = await _authService.LoginAsync(request, ipAddress);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lưu FCM Token cho push notification
    /// </summary>
    /// <param name="request">FCM token và device type</param>
    /// <returns>Kết quả lưu token</returns>
    /// <response code="200">Lưu token thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    [HttpPost("fcm-token")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveFcmToken([FromBody] FcmTokenRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BaseResponseDto
            {
                Success = false,
                Message = "Token là bắt buộc"
            });
        }

        var userId = GetUserIdFromToken();
        var result = await _authService.SaveFcmTokenAsync(userId, request);

        return Ok(result);
    }

    /// <summary>
    /// Đăng xuất
    /// </summary>
    /// <param name="request">FCM token cần xóa (optional)</param>
    /// <returns>Kết quả đăng xuất</returns>
    /// <response code="200">Đăng xuất thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto? request)
    {
        var userId = GetUserIdFromToken();
        var result = await _authService.LogoutAsync(userId, request);

        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin profile của user hiện tại
    /// </summary>
    /// <returns>Profile data</returns>
    /// <response code="200">Lấy profile thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserIdFromToken();
        var result = await _authService.GetProfileAsync(userId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Cập nhật thông tin profile của user hiện tại
    /// </summary>
    /// <param name="request">Thông tin cần cập nhật</param>
    /// <returns>Profile data đã cập nhật</returns>
    /// <response code="200">Cập nhật thành công</response>
    /// <response code="400">Validation error hoặc email đã tồn tại</response>
    /// <response code="401">Chưa đăng nhập</response>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProfileResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
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

            return BadRequest(new ProfileResponseDto
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var userId = GetUserIdFromToken();
        var result = await _authService.UpdateProfileAsync(userId, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
