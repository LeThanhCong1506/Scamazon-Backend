using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MV.PresentationLayer.Hubs;

/// <summary>
/// Hub chung cho các luồng dữ liệu realtime: Notifications, Orders, Products.
/// Client kết nối qua: ws://host/app-hub?access_token=JWT
/// </summary>
[Authorize]
public class AppHub : Hub
{
    private static readonly ConcurrentDictionary<int, HashSet<string>> _userConnections = new();

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

    /// <summary>
    /// Kiểm tra user có đang online (kết nối AppHub) không
    /// </summary>
    public static bool IsUserOnline(int userId)
    {
        return _userConnections.ContainsKey(userId);
    }

    /// <summary>
    /// Lấy danh sách connectionId của user
    /// </summary>
    public static HashSet<string>? GetUserConnections(int userId)
    {
        _userConnections.TryGetValue(userId, out var connections);
        return connections;
    }

    private int GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst("user_id");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }
}
