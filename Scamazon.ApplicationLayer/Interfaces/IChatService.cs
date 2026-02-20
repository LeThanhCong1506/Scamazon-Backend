using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

public interface IChatService
{
    Task<ChatConversationsResponseDto> GetConversationsAsync(int userId);
    Task<ChatMessagesResponseDto> GetMessagesAsync(int chatRoomId, int userId, int page, int limit, bool isAdmin = false);
    Task<SendMessageResponseDto> SendMessageAsync(int chatRoomId, int senderId, SendMessageRequestDto request, bool isFromStore);
    Task<ChatConversationsResponseDto> GetAllChatRoomsAsync();
    Task<StartChatResponseDto> StartChatAsync(int userId, int? storeId);
    Task<int> GetChatRoomOwnerIdAsync(int chatRoomId);
    Task<List<int>> GetAdminUserIdsAsync();
}
