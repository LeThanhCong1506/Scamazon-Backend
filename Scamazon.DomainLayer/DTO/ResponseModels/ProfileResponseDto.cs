namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO cho response lấy profile
/// </summary>
public class ProfileResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ProfileDataDto? Data { get; set; }
}

/// <summary>
/// Data profile của user
/// </summary>
public class ProfileDataDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Ward { get; set; }
    public DateTime? CreatedAt { get; set; }
}
