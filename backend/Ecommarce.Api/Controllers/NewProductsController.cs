namespace Ecommarce.Api.Controllers;

using Ecommarce.Application.Features.Products.Commands.CreateProduct;
using Ecommarce.Application.Features.Products.Commands.DeleteProduct;
using Ecommarce.Application.Features.Products.Queries.GetProductById;
using Ecommarce.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class ProductsController : ControllerBase
{
    private readonly Application.Services.Interfaces.IProductService _productService;

    public ProductsController(Application.Services.Interfaces.IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] string? statusTab,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        var query = new GetProductsQuery
        {
            SearchTerm = searchTerm,
            Category = category,
            StatusTab = statusTab,
            Page = page,
            PageSize = pageSize
        };

        var result = await _productService.GetProductsAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await _productService.CreateProductAsync(command);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        return Ok(new { success = result, message = "Product deleted successfully" });
    }
}
