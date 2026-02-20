using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

public interface IChatService
{
    Task<ChatConversationsResponseDto> GetConversationsAsync(int userId);
    Task<ChatMessagesResponseDto> GetMessagesAsync(int chatRoomId, int userId, int page, int limit);
    Task<SendMessageResponseDto> SendMessageAsync(int chatRoomId, int senderId, SendMessageRequestDto request, bool isFromStore);
    Task<ChatConversationsResponseDto> GetAllChatRoomsAsync();
}
