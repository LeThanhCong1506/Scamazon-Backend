using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;

namespace MV.PresentationLayer.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly INotificationService _notificationService;

    private static readonly ConcurrentDictionary<int, HashSet<string>> _userConnections = new();

    public ChatHub(IChatService chatService, INotificationService notificationService)
    {
        _chatService = chatService;
        _notificationService = notificationService;
    }

    public override Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId > 0)
        {
            _userConnections.AddOrUpdate(userId,
                _ => new HashSet<string> { Context.ConnectionId },
                (_, connections) =>
                {
                    lock (connections) { connections.Add(Context.ConnectionId); }
                    return connections;
                });
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId > 0 && _userConnections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(Context.ConnectionId);
                if (connections.Count == 0)
                {
                    _userConnections.TryRemove(userId, out _);
                }
            }
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChatRoom(int chatRoomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{chatRoomId}");
    }

    public async Task LeaveChatRoom(int chatRoomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{chatRoomId}");
    }

    public async Task SendMessage(int chatRoomId, SendMessageRequestDto request)
    {
        var userId = GetUserId();
        if (userId == 0) return;

        var isFromStore = IsAdmin();
        var result = await _chatService.SendMessageAsync(chatRoomId, userId, request, isFromStore);

        if (result.Success && result.Data != null)
        {
            await Clients.Group($"chat_{chatRoomId}").SendAsync("ReceiveMessage", result.Data);

            // Push notification if recipient is offline
            var recipientUserId = await GetRecipientUserId(chatRoomId, userId);
            if (recipientUserId > 0 && !IsUserOnline(recipientUserId))
            {
                try
                {
                    await _notificationService.SendChatNotificationAsync(
                        recipientUserId,
                        result.Data.SenderName ?? "Người dùng",
                        result.Data.Content,
                        chatRoomId);
                }
                catch
                {
                    // Don't fail message send if notification fails
                }
            }
        }
    }

    public static bool IsUserOnline(int userId)
    {
        return _userConnections.ContainsKey(userId);
    }

    private int GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst("user_id");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    private bool IsAdmin()
    {
        return Context.User?.IsInRole("admin") == true;
    }

    private async Task<int> GetRecipientUserId(int chatRoomId, int senderId)
    {
        // Get conversations to find the room owner
        var conversations = await _chatService.GetAllChatRoomsAsync();
        var room = conversations.Data?.FirstOrDefault(r => r.Id == chatRoomId);
        if (room == null) return 0;

        // If sender is the room owner (customer), notify admin (userId=0 means broadcast)
        // If sender is admin, notify the room owner (customer)
        return room.UserId == senderId ? 0 : room.UserId;
    }
}
