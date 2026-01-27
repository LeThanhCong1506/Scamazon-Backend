namespace MV.DomainLayer.DTO.RequestModels;

/// <summary>
/// DTO cho request logout (optional)
/// </summary>
public class LogoutRequestDto
{
    public string? FcmToken { get; set; }
}
