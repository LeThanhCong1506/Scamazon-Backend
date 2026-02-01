using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO cho request tạo review sản phẩm
/// </summary>
public class ReviewRequestDto
{
    [Required(ErrorMessage = "Rating là bắt buộc")]
    [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5 sao")]
    public int Rating { get; set; }

    [MaxLength(1000, ErrorMessage = "Comment không được quá 1000 ký tự")]
    public string? Comment { get; set; }
}
