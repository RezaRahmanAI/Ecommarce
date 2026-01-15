using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController : ControllerBase
{
    private readonly IAdminCatalogService _catalogService;

    public AdminCategoriesController(IAdminCatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public IActionResult GetCategories()
    {
        return Ok(_catalogService.GetCategories());
    }

    [HttpGet("tree")]
    public IActionResult GetCategoryTree()
    {
        return Ok(_catalogService.GetCategoryTree());
    }

    [HttpGet("{id}")]
    public IActionResult GetCategory(string id)
    {
        var category = _catalogService.GetCategory(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public IActionResult CreateCategory(CategoryPayload payload)
    {
        return Ok(_catalogService.CreateCategory(payload));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCategory(string id, CategoryPayload payload)
    {
        var category = _catalogService.UpdateCategory(id, payload);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(string id)
    {
        return Ok(_catalogService.DeleteCategory(id));
    }

    [HttpPost("reorder")]
    public IActionResult ReorderCategories(ReorderPayload payload)
    {
        return Ok(_catalogService.ReorderCategories(payload));
    }

    [HttpPost("image")]
    public async Task<IActionResult> UploadCategoryImage()
    {
        if (!Request.HasFormContentType)
        {
            return BadRequest("Expected multipart form data.");
        }

        var form = await Request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();
        if (file is null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var base64 = Convert.ToBase64String(stream.ToArray());
        var dataUrl = $"data:{file.ContentType};base64,{base64}";
        return Ok(dataUrl);
    }
}
