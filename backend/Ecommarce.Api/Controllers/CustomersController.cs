using Ecommarce.Api.Data;
using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly CustomerOrderStore _store;

    public CustomersController(CustomerOrderStore store)
    {
        _store = store;
    }

    [HttpGet("lookup")]
    public IActionResult Lookup([FromQuery] string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return BadRequest("Phone number is required.");
        }

        var profile = _store.GetProfile(phone);
        if (profile is null)
        {
            return NotFound();
        }

        return Ok(new CustomerLookupResponse
        {
            Name = profile.Name,
            Phone = profile.Phone,
            Address = profile.Address
        });
    }
}
