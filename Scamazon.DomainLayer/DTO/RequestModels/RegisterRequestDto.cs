using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO cho request đăng ký tài khoản
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// Username - bắt buộc, 3-50 ký tự, chỉ chữ/số/_
    /// </summary>
    [Required(ErrorMessage = "Username là bắt buộc")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username phải từ 3-50 ký tự")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username chỉ được chứa chữ cái, số và dấu gạch dưới")]
    public string Username { get; set; } = null!;

    /// <summary>
    /// Password - bắt buộc, tối thiểu 6 ký tự
    /// </summary>
    [Required(ErrorMessage = "Password là bắt buộc")]
    [MinLength(6, ErrorMessage = "Password phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Email - tùy chọn, format email hợp lệ
    /// </summary>
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    public string? Email { get; set; }

    /// <summary>
    /// Phone - tùy chọn, 10-11 chữ số
    /// </summary>
    [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Số điện thoại phải có 10-11 chữ số")]
    public string? Phone { get; set; }

    /// <summary>
    /// Full name - tùy chọn
    /// </summary>
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string? FullName { get; set; }

    /// <summary>
    /// Address - tùy chọn
    /// </summary>
    [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
    public string? Address { get; set; }

    /// <summary>
    /// City - tùy chọn
    /// </summary>
    [StringLength(100, ErrorMessage = "Thành phố không được vượt quá 100 ký tự")]
    public string? City { get; set; }

    /// <summary>
    /// District - tùy chọn
    /// </summary>
    [StringLength(100, ErrorMessage = "Quận/Huyện không được vượt quá 100 ký tự")]
    public string? District { get; set; }

    /// <summary>
    /// Ward - tùy chọn
    /// </summary>
    [StringLength(100, ErrorMessage = "Phường/Xã không được vượt quá 100 ký tự")]
    public string? Ward { get; set; }
}
