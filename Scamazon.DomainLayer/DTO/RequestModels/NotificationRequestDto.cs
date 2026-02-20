using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

public class SendNotificationRequestDto
{
    [Required(ErrorMessage = "UserId là bắt buộc")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    public string Type { get; set; } = "general";

    public string? Data { get; set; }
}
