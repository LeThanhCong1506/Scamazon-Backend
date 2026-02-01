namespace MV.DomainLayer.DTO.ResponseModels;

/// <summary>
/// DTO response cho danh sách brands
/// </summary>
public class BrandListResponseDto
{
    public bool Success { get; set; }
    public List<BrandDto> Data { get; set; } = new();
}

/// <summary>
/// DTO cho một brand
/// </summary>
public class BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
}