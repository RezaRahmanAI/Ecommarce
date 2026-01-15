using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/settings")]
public sealed class AdminSettingsController : ControllerBase
{
    private readonly IAdminSettingsService _settingsService;

    public AdminSettingsController(IAdminSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public IActionResult GetSettings()
    {
        return Ok(_settingsService.GetSettings());
    }

    [HttpPut]
    public IActionResult SaveSettings(AdminSettings payload)
    {
        return Ok(_settingsService.SaveSettings(payload));
    }

    [HttpPost("shipping-zones")]
    public IActionResult CreateShippingZone(ShippingZone payload)
    {
        return Ok(_settingsService.CreateShippingZone(payload));
    }

    [HttpPut("shipping-zones/{id:int}")]
    public IActionResult UpdateShippingZone(int id, ShippingZone payload)
    {
        var zone = _settingsService.UpdateShippingZone(id, payload);
        return zone is null ? NotFound() : Ok(zone);
    }

    [HttpDelete("shipping-zones/{id:int}")]
    public IActionResult DeleteShippingZone(int id)
    {
        return Ok(_settingsService.DeleteShippingZone(id));
    }
}
