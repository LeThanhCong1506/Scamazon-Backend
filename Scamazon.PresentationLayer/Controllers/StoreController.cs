using Microsoft.AspNetCore.Mvc;
using MV.ApplicationLayer.Interfaces;

namespace MV.PresentationLayer.Controllers;

[Route("api/store")]
[ApiController]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetStoreInfo()
    {
        var result = await _storeService.GetStoreInfoAsync();
        return Ok(result);
    }
}
