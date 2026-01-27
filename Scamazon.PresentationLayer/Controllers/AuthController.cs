using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller xử lý Authentication (Register, Login)
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
}
