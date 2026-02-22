namespace MV.DomainLayer.DTO.ResponseModels;

// ==================== FAVORITE ====================

/// <summary>
/// Response danh sách yêu thích
/// </summary>
public class FavoriteListResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<FavoriteItemDto>? Data { get; set; }
}

public class FavoriteItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductSlug { get; set; } = null!;
    public string? ProductImage { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Response toggle favorite
/// </summary>
public class FavoriteToggleResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public FavoriteToggleDataDto? Data { get; set; }
}

public class FavoriteToggleDataDto
{
    public bool IsFavorited { get; set; }
    public int ProductId { get; set; }
}

/// <summary>
/// Response danh sách product ID yêu thích
/// </summary>
public class FavoriteIdsResponseDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<int>? Data { get; set; }
}
