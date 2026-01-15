using Ecommarce.Api.Data;
using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController : ControllerBase
{
    private readonly AdminDataStore _store;

    public AdminCategoriesController(AdminDataStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetCategories()
    {
        return Ok(_store.GetCategories());
    }

    [HttpGet("tree")]
    public IActionResult GetCategoryTree()
    {
        return Ok(_store.GetCategoryTree());
    }

    [HttpGet("{id}")]
    public IActionResult GetCategory(string id)
    {
        var category = _store.GetCategory(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public IActionResult CreateCategory(CategoryPayload payload)
    {
        return Ok(_store.CreateCategory(payload));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCategory(string id, CategoryPayload payload)
    {
        var category = _store.UpdateCategory(id, payload);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(string id)
    {
        return Ok(_store.DeleteCategory(id));
    }

    [HttpPost("reorder")]
    public IActionResult ReorderCategories(ReorderPayload payload)
    {
        return Ok(_store.ReorderCategories(payload));
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
