namespace MV.DomainLayer.DTO.ResponseModels;

public class NotificationListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public List<NotificationDto>? Data { get; set; }
}

public class NotificationDto
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Body { get; set; }
    public string? Data { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? CreatedAt { get; set; }
}
