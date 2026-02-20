namespace MV.DomainLayer.DTO.ResponseModels;

public class StoreInfoResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public List<StoreDataDto>? Data { get; set; }
}

public class StoreDataDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Phone { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? OpeningHours { get; set; }
    public bool? IsActive { get; set; }
}
