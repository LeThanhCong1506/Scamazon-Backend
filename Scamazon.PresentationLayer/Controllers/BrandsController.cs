using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller xử lý thương hiệu
/// </summary>
[Route("api/brands")]
[ApiController]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    /// <summary>
    /// Lấy danh sách thương hiệu
    /// </summary>
    /// <returns>Danh sách thương hiệu</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    [HttpGet]
    [ProducesResponseType(typeof(BrandListResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBrands()
    {
        var result = await _brandService.GetBrandsAsync();
        return Ok(result);
    }
}