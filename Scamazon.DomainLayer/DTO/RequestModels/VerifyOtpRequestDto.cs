using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

public class VerifyOtpRequestDto
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mã OTP là bắt buộc")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải đúng 6 chữ số")]
    public string Otp { get; set; } = null!;
}
