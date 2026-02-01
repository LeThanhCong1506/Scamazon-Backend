using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

/// <summary>
/// Interface cho Brand Service
/// </summary>
public interface IBrandService
{
    /// <summary>
    /// Lấy danh sách brands
    /// </summary>
    Task<BrandListResponseDto> GetBrandsAsync();
}