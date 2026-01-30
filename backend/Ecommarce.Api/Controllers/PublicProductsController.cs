namespace Ecommarce.Api.Controllers;

using Ecommarce.Application.Features.Products.Queries.GetProductById;
using Ecommarce.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
public class PublicProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all products with optional filters (Public - No Auth Required)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] int? page = 1,
        [FromQuery] int? pageSize = 20)
    {
        var query = new GetProductsQuery
        {
            SearchTerm = searchTerm,
            Category = category,
            StatusTab = "Active", // Only show active products to public
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get product by ID (Public - No Auth Required)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get featured products (Public - No Auth Required)
    /// </summary>
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProducts([FromQuery] int? limit = 10)
    {
        var query = new GetProductsQuery
        {
            StatusTab = "Active",
            Page = 1,
            PageSize = limit
        };

        var result = await _mediator.Send(query);
        
        // Filter only featured products
        var featuredProducts = result.Where(p => p.Featured).ToList();
        
        return Ok(featuredProducts);
    }

    /// <summary>
    /// Get new arrival products (Public - No Auth Required)
    /// </summary>
    [HttpGet("new-arrivals")]
    public async Task<IActionResult> GetNewArrivals([FromQuery] int? limit = 10)
    {
        var query = new GetProductsQuery
        {
            StatusTab = "Active",
            Page = 1,
            PageSize = limit
        };

        var result = await _mediator.Send(query);
        
        // Filter only new arrivals
        var newArrivals = result.Where(p => p.NewArrival).ToList();
        
        return Ok(newArrivals);
    }
}
