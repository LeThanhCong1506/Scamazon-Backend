using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

/// <summary>
/// Interface cho Authentication Service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
}
