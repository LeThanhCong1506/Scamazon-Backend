namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO cho response authentication (Register/Login)
/// </summary>
public class AuthResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public AuthDataDto? Data { get; set; }
    public List<ValidationErrorDto>? Errors { get; set; }
}

/// <summary>
/// Data trả về khi auth thành công
/// </summary>
public class AuthDataDto
{
    public UserResponseDto User { get; set; } = null!;
    public string Token { get; set; } = null!;
}

/// <summary>
/// Thông tin user trả về cho client
/// </summary>
public class UserResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? FullName { get; set; }
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Chi tiết lỗi validation
/// </summary>
public class ValidationErrorDto
{
    public string Field { get; set; } = null!;
    public string Message { get; set; } = null!;
}
