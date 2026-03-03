using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

public class ForgotPasswordRequestDto
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;
}
