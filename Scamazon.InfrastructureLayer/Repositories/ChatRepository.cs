using Microsoft.EntityFrameworkCore;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.DBContexts;
using MV.InfrastructureLayer.Interfaces;

namespace MV.InfrastructureLayer.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly ScamazonDbContext _context;

    public ChatRepository(ScamazonDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChatRoom>> GetChatRoomsByUserIdAsync(int userId)
    {
        return await _context.ChatRooms
            .Include(r => r.User)
            .Include(r => r.Store)
            .Include(r => r.ChatMessages.OrderByDescending(m => m.CreatedAt).Take(1))
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.LastMessageAt ?? r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ChatRoom>> GetAllChatRoomsAsync()
    {
        return await _context.ChatRooms
            .Include(r => r.User)
            .Include(r => r.Store)
            .Include(r => r.ChatMessages.OrderByDescending(m => m.CreatedAt).Take(1))
            .OrderByDescending(r => r.LastMessageAt ?? r.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChatRoom?> GetOrCreateChatRoomAsync(int userId, int? storeId)
    {
        var room = await _context.ChatRooms
            .Include(r => r.User)
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.StoreId == storeId);

        if (room != null) return room;

        room = new ChatRoom
        {
            UserId = userId,
            StoreId = storeId,
            Status = "active",
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatRooms.Add(room);
        await _context.SaveChangesAsync();

        // Reload with includes
        return await _context.ChatRooms
            .Include(r => r.User)
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.Id == room.Id);
    }

    public async Task<ChatRoom?> GetChatRoomByIdAsync(int chatRoomId)
    {
        return await _context.ChatRooms
            .Include(r => r.User)
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.Id == chatRoomId);
    }

    public async Task<List<ChatMessage>> GetMessagesByChatRoomIdAsync(int chatRoomId, int page, int limit)
    {
        return await _context.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Product)
                .ThenInclude(p => p!.ProductImages)
            .Where(m => m.ChatRoomId == chatRoomId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<ChatMessage> CreateMessageAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        // Reload with includes
        return await _context.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Product)
                .ThenInclude(p => p!.ProductImages)
            .FirstAsync(m => m.Id == message.Id);
    }

    public async Task UpdateLastMessageAtAsync(int chatRoomId, DateTime timestamp)
    {
        await _context.ChatRooms
            .Where(r => r.Id == chatRoomId)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.LastMessageAt, timestamp));
    }

    public async Task MarkMessagesAsReadAsync(int chatRoomId, int userId)
    {
        await _context.ChatMessages
            .Where(m => m.ChatRoomId == chatRoomId && m.SenderId != userId && m.IsRead == false)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true));
    }

    public async Task<int> CountUnreadMessagesAsync(int chatRoomId, int userId)
    {
        return await _context.ChatMessages
            .CountAsync(m => m.ChatRoomId == chatRoomId && m.SenderId != userId && m.IsRead == false);
    }
}
