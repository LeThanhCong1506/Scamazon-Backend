using System.ComponentModel.DataAnnotations;

namespace MV.DomainLayer.DTO.RequestModels;

public class SendMessageRequestDto
{
    [Required(ErrorMessage = "Nội dung tin nhắn là bắt buộc")]
    public string Content { get; set; } = null!;

    public string MessageType { get; set; } = "text";

    public string? ImageUrl { get; set; }

    public int? ProductId { get; set; }
}
