using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO cho request lưu FCM token
/// </summary>
public class FcmTokenRequestDto
{
    [Required(ErrorMessage = "Token là bắt buộc")]
    public string Token { get; set; } = null!;

    public string? DeviceType { get; set; }
}
