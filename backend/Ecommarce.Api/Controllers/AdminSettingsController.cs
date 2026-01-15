using Ecommarce.Api.Data;
using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/settings")]
public sealed class AdminSettingsController : ControllerBase
{
    private readonly AdminDataStore _store;

    public AdminSettingsController(AdminDataStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetSettings()
    {
        return Ok(_store.GetSettings());
    }

    [HttpPut]
    public IActionResult SaveSettings(AdminSettings payload)
    {
        return Ok(_store.SaveSettings(payload));
    }

    [HttpPost("shipping-zones")]
    public IActionResult CreateShippingZone(ShippingZone payload)
    {
        return Ok(_store.CreateShippingZone(payload));
    }

    [HttpPut("shipping-zones/{id:int}")]
    public IActionResult UpdateShippingZone(int id, ShippingZone payload)
    {
        var zone = _store.UpdateShippingZone(id, payload);
        return zone is null ? NotFound() : Ok(zone);
    }

    [HttpDelete("shipping-zones/{id:int}")]
    public IActionResult DeleteShippingZone(int id)
    {
        return Ok(_store.DeleteShippingZone(id));
    }
}
