namespace Ecommarce.Api.Controllers;

using Ecommarce.Application.Features.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly Application.Services.Interfaces.ICategoryService _categoryService;

    public CategoriesController(Application.Services.Interfaces.ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();
        return Ok(result);
    }
}
