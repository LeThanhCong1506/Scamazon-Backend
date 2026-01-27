using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO cho request cập nhật profile
/// </summary>
public class UpdateProfileRequestDto
{
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
    public string? Phone { get; set; }

    [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    public string? FullName { get; set; }

    [MaxLength(500, ErrorMessage = "URL avatar tối đa 500 ký tự")]
    public string? AvatarUrl { get; set; }

    public string? Address { get; set; }

    [MaxLength(100, ErrorMessage = "Thành phố tối đa 100 ký tự")]
    public string? City { get; set; }

    [MaxLength(100, ErrorMessage = "Quận/Huyện tối đa 100 ký tự")]
    public string? District { get; set; }

    [MaxLength(100, ErrorMessage = "Phường/Xã tối đa 100 ký tự")]
    public string? Ward { get; set; }
}
