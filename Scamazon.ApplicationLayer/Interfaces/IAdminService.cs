using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.ApplicationLayer.Interfaces;

/// <summary>
/// Interface cho Admin Service
/// </summary>
public interface IAdminService
{
    // Dashboard
    Task<DashboardStatsResponseDto> GetDashboardStatsAsync();

    // Category CRUD
    Task<BaseResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request, int adminId, string? ipAddress);
    Task<BaseResponseDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto request, int adminId, string? ipAddress);
    Task<BaseResponseDto> DeleteCategoryAsync(int id, int adminId, string? ipAddress);

    // Brand CRUD
    Task<BaseResponseDto> CreateBrandAsync(CreateBrandRequestDto request, int adminId, string? ipAddress);
    Task<BaseResponseDto> UpdateBrandAsync(int id, UpdateBrandRequestDto request, int adminId, string? ipAddress);
    Task<BaseResponseDto> DeleteBrandAsync(int id, int adminId, string? ipAddress);

    // Product CRUD
    Task<BaseResponseDto> CreateProductAsync(CreateProductRequestDto request, int adminId, string? ipAddress);
    Task<BaseResponseDto> UpdateProductAsync(int id, UpdateProductRequestDto request, int adminId, string? ipAddress);
    Task<BaseResponseDto> DeleteProductAsync(int id, int adminId, string? ipAddress);
}
