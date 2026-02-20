using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<ChatConversationsResponseDto> GetConversationsAsync(int userId)
    {
        var rooms = await _chatRepository.GetChatRoomsByUserIdAsync(userId);

        var data = new List<ChatRoomSummaryDto>();
        foreach (var room in rooms)
        {
            var unreadCount = await _chatRepository.CountUnreadMessagesAsync(room.Id, userId);
            var lastMsg = room.ChatMessages.FirstOrDefault();

            data.Add(new ChatRoomSummaryDto
            {
                Id = room.Id,
                UserId = room.UserId,
                UserName = room.User?.FullName ?? room.User?.Username,
                UserAvatar = room.User?.AvatarUrl,
                StoreId = room.StoreId,
                StoreName = room.Store?.Name,
                Status = room.Status,
                LastMessage = lastMsg?.Content,
                LastMessageAt = room.LastMessageAt,
                UnreadCount = unreadCount,
                CreatedAt = room.CreatedAt
            });
        }

        return new ChatConversationsResponseDto
        {
            Success = true,
            Message = "Lấy danh sách cuộc trò chuyện thành công",
            Data = data
        };
    }

    public async Task<ChatMessagesResponseDto> GetMessagesAsync(int chatRoomId, int userId, int page, int limit)
    {
        var room = await _chatRepository.GetChatRoomByIdAsync(chatRoomId);
        if (room == null)
        {
            return new ChatMessagesResponseDto
            {
                Success = false,
                Message = "Không tìm thấy phòng chat"
            };
        }

        // Mark messages as read
        await _chatRepository.MarkMessagesAsReadAsync(chatRoomId, userId);

        var messages = await _chatRepository.GetMessagesByChatRoomIdAsync(chatRoomId, page, limit);

        var messageDtos = messages.Select(MapToMessageDto).ToList();

        return new ChatMessagesResponseDto
        {
            Success = true,
            Message = "Lấy tin nhắn thành công",
            Data = new ChatMessagesDataDto
            {
                ChatRoomId = chatRoomId,
                Messages = messageDtos
            }
        };
    }

    public async Task<SendMessageResponseDto> SendMessageAsync(int chatRoomId, int senderId, SendMessageRequestDto request, bool isFromStore)
    {
        var room = await _chatRepository.GetChatRoomByIdAsync(chatRoomId);
        if (room == null)
        {
            return new SendMessageResponseDto
            {
                Success = false,
                Message = "Không tìm thấy phòng chat"
            };
        }

        var message = new ChatMessage
        {
            ChatRoomId = chatRoomId,
            SenderId = senderId,
            MessageType = request.MessageType,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            ProductId = request.ProductId,
            IsFromStore = isFromStore,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        var savedMessage = await _chatRepository.CreateMessageAsync(message);
        await _chatRepository.UpdateLastMessageAtAsync(chatRoomId, savedMessage.CreatedAt ?? DateTime.UtcNow);

        return new SendMessageResponseDto
        {
            Success = true,
            Message = "Gửi tin nhắn thành công",
            Data = MapToMessageDto(savedMessage)
        };
    }

    public async Task<ChatConversationsResponseDto> GetAllChatRoomsAsync()
    {
        var rooms = await _chatRepository.GetAllChatRoomsAsync();

        var data = new List<ChatRoomSummaryDto>();
        foreach (var room in rooms)
        {
            var lastMsg = room.ChatMessages.FirstOrDefault();

            data.Add(new ChatRoomSummaryDto
            {
                Id = room.Id,
                UserId = room.UserId,
                UserName = room.User?.FullName ?? room.User?.Username,
                UserAvatar = room.User?.AvatarUrl,
                StoreId = room.StoreId,
                StoreName = room.Store?.Name,
                Status = room.Status,
                LastMessage = lastMsg?.Content,
                LastMessageAt = room.LastMessageAt,
                UnreadCount = 0,
                CreatedAt = room.CreatedAt
            });
        }

        return new ChatConversationsResponseDto
        {
            Success = true,
            Message = "Lấy tất cả phòng chat thành công",
            Data = data
        };
    }

    private static ChatMessageDto MapToMessageDto(ChatMessage m)
    {
        var primaryImage = m.Product?.ProductImages
            .FirstOrDefault(pi => pi.IsPrimary == true)?.ImageUrl
            ?? m.Product?.ProductImages.FirstOrDefault()?.ImageUrl;

        return new ChatMessageDto
        {
            Id = m.Id,
            ChatRoomId = m.ChatRoomId,
            SenderId = m.SenderId,
            SenderName = m.Sender?.FullName ?? m.Sender?.Username,
            MessageType = m.MessageType,
            Content = m.Content,
            ImageUrl = m.ImageUrl,
            ProductId = m.ProductId,
            ProductName = m.Product?.Name,
            ProductImage = primaryImage,
            IsFromStore = m.IsFromStore,
            IsRead = m.IsRead,
            CreatedAt = m.CreatedAt
        };
    }
}
