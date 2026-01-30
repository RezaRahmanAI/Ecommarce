using ECommerce.Core.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<dynamic>> LookupCustomer([FromQuery] string phone)
    {
        // Simple lookup by phone from existing orders to find returning customer
        var lastOrder = await _context.Orders
            .Where(o => o.CustomerPhone == phone)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastOrder == null) return NotFound();

        return Ok(new
        {
            name = lastOrder.CustomerName,
            phone = lastOrder.CustomerPhone,
            address = lastOrder.ShippingAddress
        });
    }
}
