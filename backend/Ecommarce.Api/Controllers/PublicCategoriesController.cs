namespace Ecommarce.Api.Controllers;

using Ecommarce.Application.Features.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/categories")]
public class PublicCategoriesController : ControllerBase
{
    private readonly Application.Services.Interfaces.ICategoryService _categoryService;

    public PublicCategoriesController(Application.Services.Interfaces.ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories (Public - No Auth Required)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();
        return Ok(result);
    }
}
