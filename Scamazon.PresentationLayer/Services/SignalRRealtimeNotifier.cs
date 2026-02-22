using Microsoft.AspNetCore.SignalR;
using MV.ApplicationLayer.Interfaces;
using MV.PresentationLayer.Hubs;

namespace MV.PresentationLayer.Services;

/// <summary>
/// Implementation của IRealtimeNotifier sử dụng SignalR AppHub.
/// Gửi event realtime đến user qua WebSocket nếu user đang online.
/// </summary>
public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<AppHub> _hubContext;
    private readonly ILogger<SignalRRealtimeNotifier> _logger;

    public SignalRRealtimeNotifier(IHubContext<AppHub> hubContext, ILogger<SignalRRealtimeNotifier> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendToUserAsync(int userId, string eventName, object payload)
    {
        try
        {
            var connections = AppHub.GetUserConnections(userId);
            if (connections == null || connections.Count == 0) return;

            string[] connectionIds;
            lock (connections)
            {
                connectionIds = connections.ToArray();
            }

            await _hubContext.Clients.Clients(connectionIds).SendAsync(eventName, payload);
            _logger.LogInformation("Sent realtime '{Event}' to user {UserId} ({Count} connections)",
                eventName, userId, connectionIds.Length);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send realtime '{Event}' to user {UserId}", eventName, userId);
        }
    }
}
