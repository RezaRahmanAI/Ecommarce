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
                ParentId = c.ParentId,
                IsVisible = c.IsVisible,
                SortOrder = c.SortOrder,
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
    public async Task<ActionResult<object>> UploadImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var imageUrl = await SaveImageAsync(file);
        return Ok(new { url = imageUrl });
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required");

        // Generate slug if not provided
        var slug = string.IsNullOrWhiteSpace(dto.Slug) ? GenerateSlug(dto.Name) : dto.Slug;

        var category = new Category
        {
            Name = dto.Name,
            Slug = slug,
            ImageUrl = dto.ImageUrl,
            ParentId = dto.ParentId,
            IsVisible = dto.IsVisible ?? true,
            SortOrder = dto.SortOrder ?? 0
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ImageUrl = category.ImageUrl,
            ParentId = category.ParentId,
            IsVisible = category.IsVisible,
            SortOrder = category.SortOrder,
            ProductCount = 0,
            CreatedAt = category.CreatedAt
        };

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required");

        category.Name = dto.Name;
        category.Slug = string.IsNullOrWhiteSpace(dto.Slug) ? GenerateSlug(dto.Name) : dto.Slug;
        category.ParentId = dto.ParentId;
        category.IsVisible = dto.IsVisible ?? category.IsVisible;
        category.SortOrder = dto.SortOrder ?? category.SortOrder;
        category.UpdatedAt = DateTime.UtcNow;

        // Update image URL if provided
        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            // Delete old image if it's different and exists
            if (!string.IsNullOrEmpty(category.ImageUrl) && category.ImageUrl != dto.ImageUrl)
            {
                DeleteImage(category.ImageUrl);
            }
            category.ImageUrl = dto.ImageUrl;
        }

        await _context.SaveChangesAsync();

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ImageUrl = category.ImageUrl,
            ParentId = category.ParentId,
            IsVisible = category.IsVisible,
            SortOrder = category.SortOrder,
            ProductCount = await _context.Products.CountAsync(p => p.CategoryId == id),
            CreatedAt = category.CreatedAt
        };

        return Ok(result);
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

    [HttpPost("reorder")]
    public async Task<ActionResult<bool>> ReorderCategories([FromBody] ReorderCategoriesDto dto)
    {
        if (dto.OrderedIds == null || dto.OrderedIds.Count == 0)
            return BadRequest("OrderedIds is required");

        var categories = await _context.Categories
            .Where(c => (c.ParentId ?? 0) == (dto.ParentId ?? 0))
            .ToListAsync();

        for (int i = 0; i < dto.OrderedIds.Count; i++)
        {
            var category = categories.FirstOrDefault(c => c.Id == dto.OrderedIds[i]);
            if (category != null)
            {
                category.SortOrder = i + 1;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(true);
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
