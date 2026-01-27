using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO cho request đăng nhập
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "Username là bắt buộc")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password là bắt buộc")]
    public string Password { get; set; } = null!;
}
