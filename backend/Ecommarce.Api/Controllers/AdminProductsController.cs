using Ecommarce.Api.Data;
using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/products")]
public sealed class AdminProductsController : ControllerBase
{
    private readonly AdminDataStore _store;

    public AdminProductsController(AdminDataStore store)
    {
        _store = store;
    }

    [HttpGet("catalog")]
    public IActionResult GetCatalog()
    {
        return Ok(_store.GetProducts());
    }

    [HttpGet]
    public IActionResult GetProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] string? statusTab,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (items, total) = _store.FilterProducts(searchTerm ?? string.Empty, category ?? string.Empty, statusTab ?? string.Empty, page, pageSize);
        return Ok(new { items, total });
    }

    [HttpGet("filtered")]
    public IActionResult GetFilteredProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] string? statusTab)
    {
        var items = _store.FilterProducts(searchTerm ?? string.Empty, category ?? string.Empty, statusTab ?? string.Empty);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetProduct(int id)
    {
        var product = _store.GetProduct(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public IActionResult CreateProduct(ProductCreatePayload payload)
    {
        return Ok(_store.CreateProduct(payload));
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateProduct(int id, ProductUpdatePayload payload)
    {
        var product = _store.UpdateProduct(id, payload);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteProduct(int id)
    {
        return Ok(_store.DeleteProduct(id));
    }

    [HttpPost("media")]
    public async Task<IActionResult> UploadMedia()
    {
        if (!Request.HasFormContentType)
        {
            return BadRequest("Expected multipart form data.");
        }

        var form = await Request.ReadFormAsync();
        var files = form.Files;
        if (files.Count == 0)
        {
            return Ok(Array.Empty<string>());
        }

        var results = new List<string>();
        foreach (var file in files)
        {
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var base64 = Convert.ToBase64String(stream.ToArray());
            results.Add($"data:{file.ContentType};base64,{base64}");
        }

        return Ok(results);
    }

    [HttpPost("{id:int}/media/remove")]
    public IActionResult RemoveMedia(int id, ProductMediaRemovePayload payload)
    {
        return Ok(_store.RemoveProductMedia(id, payload.MediaUrl));
    }
}
