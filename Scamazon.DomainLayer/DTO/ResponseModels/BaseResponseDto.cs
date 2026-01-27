namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO base cho response đơn giản
/// </summary>
public class BaseResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
}
