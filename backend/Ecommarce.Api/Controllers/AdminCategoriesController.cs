using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController : ControllerBase
{
    private readonly IAdminCatalogService _catalogService;
    private readonly IImageStorageService _imageStorage;

    public AdminCategoriesController(IAdminCatalogService catalogService, IImageStorageService imageStorage)
    {
        _catalogService = catalogService;
        _imageStorage = imageStorage;
    }

    [HttpGet]
    public IActionResult GetCategories()
    {
        var categories = _catalogService.GetCategories()
            .Select(MapCategory)
            .ToList();
        return Ok(categories);
    }

    [HttpGet("tree")]
    public IActionResult GetCategoryTree()
    {
        var tree = _catalogService.GetCategoryTree()
            .Select(MapCategoryNode)
            .ToList();
        return Ok(tree);
    }

    [HttpGet("{id}")]
    public IActionResult GetCategory(string id)
    {
        var category = _catalogService.GetCategory(id);
        return category is null ? NotFound() : Ok(MapCategory(category));
    }

    [HttpPost]
    public IActionResult CreateCategory(CategoryPayload payload)
    {
        return Ok(MapCategory(_catalogService.CreateCategory(payload)));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCategory(string id, CategoryPayload payload)
    {
        var category = _catalogService.UpdateCategory(id, payload);
        return category is null ? NotFound() : Ok(MapCategory(category));
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

        var fileName = await _imageStorage.SaveAsync(file, HttpContext.RequestAborted);
        var url = _imageStorage.BuildPublicUrl(Request, fileName);
        return Ok(url);
    }

    private Category MapCategory(Category category)
    {
        return new Category
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ParentId = category.ParentId,
            Description = category.Description,
            ImageUrl = _imageStorage.BuildPublicUrl(Request, category.ImageUrl),
            IsVisible = category.IsVisible,
            ProductCount = category.ProductCount,
            SortOrder = category.SortOrder
        };
    }

    private CategoryNode MapCategoryNode(CategoryNode node)
    {
        var children = node.Children.Select(MapCategoryNode).ToList();
        return new CategoryNode(MapCategory(node.Category), children);
    }
}
