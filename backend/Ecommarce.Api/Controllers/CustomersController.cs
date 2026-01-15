using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("lookup")]
    public IActionResult Lookup([FromQuery] string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return BadRequest("Phone number is required.");
        }

        var profile = _customerService.GetProfile(phone);
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
