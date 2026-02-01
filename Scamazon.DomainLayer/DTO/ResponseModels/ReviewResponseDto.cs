namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO response cho danh sách reviews
/// </summary>
public class ReviewListResponseDto
{
    public bool Success { get; set; }
    public ReviewListDataDto? Data { get; set; }
    public string? Message { get; set; }
}

public class ReviewListDataDto
{
    public List<ReviewDto> Reviews { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

/// <summary>
/// DTO cho một review
/// </summary>
public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public ReviewUserDto User { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
}

public class ReviewUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
}

/// <summary>
/// DTO response cho tạo review
/// </summary>
public class CreateReviewResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public ReviewDto? Data { get; set; }
}