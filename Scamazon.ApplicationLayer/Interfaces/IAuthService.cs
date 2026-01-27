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

    /// <summary>
    /// Đăng nhập
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress);

    /// <summary>
    /// Lưu FCM token
    /// </summary>
    Task<BaseResponseDto> SaveFcmTokenAsync(int userId, FcmTokenRequestDto request);

    /// <summary>
    /// Đăng xuất
    /// </summary>
    Task<BaseResponseDto> LogoutAsync(int userId, LogoutRequestDto? request);

    /// <summary>
    /// Lấy profile của user
    /// </summary>
    Task<ProfileResponseDto> GetProfileAsync(int userId);

    /// <summary>
    /// Cập nhật profile của user
    /// </summary>
    Task<ProfileResponseDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
}
