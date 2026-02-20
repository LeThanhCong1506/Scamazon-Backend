using MV.DomainLayer.Entities;

namespace MV.InfrastructureLayer.Interfaces;

public interface IChatRepository
{
    Task<List<ChatRoom>> GetChatRoomsByUserIdAsync(int userId);
    Task<List<ChatRoom>> GetAllChatRoomsAsync();
    Task<ChatRoom?> GetOrCreateChatRoomAsync(int userId, int? storeId);
    Task<ChatRoom?> GetChatRoomByIdAsync(int chatRoomId);
    Task<List<ChatMessage>> GetMessagesByChatRoomIdAsync(int chatRoomId, int page, int limit);
    Task<ChatMessage> CreateMessageAsync(ChatMessage message);
    Task UpdateLastMessageAtAsync(int chatRoomId, DateTime timestamp);
    Task MarkMessagesAsReadAsync(int chatRoomId, int userId);
    Task<int> CountUnreadMessagesAsync(int chatRoomId, int userId);
}
