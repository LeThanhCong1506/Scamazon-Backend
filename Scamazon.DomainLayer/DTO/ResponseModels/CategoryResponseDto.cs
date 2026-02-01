namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO response cho danh sách categories
/// </summary>
public class CategoryListResponseDto
{
    public bool Success { get; set; }
    public List<CategoryDto> Data { get; set; } = new();
}

/// <summary>
/// DTO cho một category
/// </summary>
public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }
    public List<CategoryDto> Children { get; set; } = new();
}