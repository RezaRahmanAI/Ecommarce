using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/categories")]
//[Authorize(Roles = "admin")]
public class AdminCategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AdminCategoryController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAllCategories()
    {
        var categories = await _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                ImageUrl = c.ImageUrl,
                ProductCount = c.Products.Count,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ImageUrl = category.ImageUrl,
            ProductCount = category.Products.Count,
            CreatedAt = category.CreatedAt
        };

        return Ok(dto);
    }

    [HttpPost("image")]
    public async Task<ActionResult<string>> UploadImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var imageUrl = await SaveImageAsync(file);
        return Ok(imageUrl);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromForm] string name, [FromForm] IFormFile? image)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Category name is required");

        // Generate slug
        var slug = GenerateSlug(name);

        // Handle image upload
        string? imageUrl = null;
        if (image != null && image.Length > 0)
        {
            imageUrl = await SaveImageAsync(image);
        }

        var category = new Category
        {
            Name = name,
            Slug = slug,
            ImageUrl = imageUrl,
            IsVisible = true,
            SortOrder = 0
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ImageUrl = category.ImageUrl,
            ProductCount = 0,
            CreatedAt = category.CreatedAt
        };

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromForm] string name, [FromForm] IFormFile? image)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Category name is required");

        category.Name = name;
        category.Slug = GenerateSlug(name);
        category.UpdatedAt = DateTime.UtcNow;

        // Handle image upload
        if (image != null && image.Length > 0)
        {
            // Delete old image if exists
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                DeleteImage(category.ImageUrl);
            }

            category.ImageUrl = await SaveImageAsync(image);
        }

        await _context.SaveChangesAsync();

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ImageUrl = category.ImageUrl,
            ProductCount = await _context.Products.CountAsync(p => p.CategoryId == id),
            CreatedAt = category.CreatedAt
        };

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        // Delete image if exists
        if (!string.IsNullOrEmpty(category.ImageUrl))
        {
            DeleteImage(category.ImageUrl);
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        // Create uploads directory if it doesn't exist
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "categories");
        Directory.CreateDirectory(uploadsFolder);

        // Generate unique filename
        var fileExtension = Path.GetExtension(image.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        // Return relative URL
        return $"/uploads/categories/{fileName}";
    }

    private void DeleteImage(string imageUrl)
    {
        try
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "categories", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        catch
        {
            // Log error but don't fail the request
        }
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLower()
            .Replace(" ", "-")
            .Replace("?", "")
            .Replace("!", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("'", "")
            .Replace("\"", "");
    }
}
