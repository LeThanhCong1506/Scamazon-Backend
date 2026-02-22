namespace MV.ApplicationLayer.Interfaces;

/// <summary>
/// Interface để gửi notification realtime qua SignalR.
/// Implementation nằm ở PresentationLayer (sử dụng IHubContext).
/// </summary>
public interface IRealtimeNotifier
{
    /// <summary>
    /// Gửi event đến user qua SignalR nếu user đang online.
    /// </summary>
    Task SendToUserAsync(int userId, string eventName, object payload);
}
