namespace MV.DomainLayer.DTO.ResponseModels;

public class ChatConversationsResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public List<ChatRoomSummaryDto>? Data { get; set; }
}

public class ChatRoomSummaryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserAvatar { get; set; }
    public int? StoreId { get; set; }
    public string? StoreName { get; set; }
    public string? Status { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class ChatMessagesResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public ChatMessagesDataDto? Data { get; set; }
}

public class ChatMessagesDataDto
{
    public int ChatRoomId { get; set; }
    public List<ChatMessageDto> Messages { get; set; } = new();
}

public class ChatMessageDto
{
    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public int? SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? MessageType { get; set; }
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
    public bool? IsFromStore { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class SendMessageResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public ChatMessageDto? Data { get; set; }
}
