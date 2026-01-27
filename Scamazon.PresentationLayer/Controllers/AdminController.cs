using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.ResponseModels;
using MV.PresentationLayer.Middlewares;

namespace MV.PresentationLayer.Controllers;

/// <summary>
/// Controller xử lý các chức năng Admin
/// </summary>
[Route("api/admin")]
[ApiController]
[Authorize]
[RequireAdmin]
public class AdminController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public AdminController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Lấy thống kê dashboard cho Admin
    /// </summary>
    /// <returns>Dashboard stats</returns>
    /// <response code="200">Lấy thống kê thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền admin</response>
    [HttpGet("dashboard/stats")]
    [ProducesResponseType(typeof(DashboardStatsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _dashboardService.GetDashboardStatsAsync();
        return Ok(result);
    }
}
