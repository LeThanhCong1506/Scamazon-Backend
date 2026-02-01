using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

/// <summary>
/// Service xử lý logic cho Brand
/// </summary>
public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    /// <summary>
    /// Lấy danh sách brands
    /// </summary>
    public async Task<BrandListResponseDto> GetBrandsAsync()
    {
        var brands = await _brandRepository.GetAllActiveAsync();

        return new BrandListResponseDto
        {
            Success = true,
            Data = brands.Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                Slug = b.Slug,
                LogoUrl = b.LogoUrl,
                Description = b.Description
            }).ToList()
        };
    }
}