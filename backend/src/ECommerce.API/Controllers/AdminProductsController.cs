using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/products")]
//[Authorize(Roles = "admin")]
public class AdminProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AdminProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpPost("media")]
    public async Task<ActionResult<List<string>>> UploadProductMedia([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No files uploaded");

        var uploadedUrls = new List<string>();
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadsFolder);

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                uploadedUrls.Add($"/uploads/products/{fileName}");
            }
        }

        return Ok(uploadedUrls);
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] string? statusTab,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Sku.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(category) && category != "all")
        {
            query = query.Where(p => p.Category.Name == category);
        }

        if (!string.IsNullOrEmpty(statusTab) && statusTab != "all")
        {
            query = query.Where(p => p.Status.ToLower() == statusTab.ToLower());
        }

        var total = await query.CountAsync();
        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Sku,
                p.Price,
                p.SalePrice,
                p.PurchaseRate,
                p.Stock,
                p.Status,
                p.ImageUrl,
                Category = p.Category.Name,
                CategoryId = p.CategoryId,
                MediaUrls = p.Images.Select(i => i.Url).ToList(),
                p.CreatedAt
            })
            .ToListAsync();

        return Ok(new { items = products, total });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetProductById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        var result = new
        {
            product.Id,
            product.Name,
            product.Description,
            product.Sku,
            product.Price,
            product.SalePrice,
            product.PurchaseRate,
            product.Stock,
            product.Status,
            product.ImageUrl,
            Category = product.Category.Name,
            CategoryId = product.CategoryId,
            MediaUrls = product.Images.Select(i => i.Url).ToList(),
            product.CreatedAt
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct([FromBody] dynamic payload)
    {
        // Parse the complex payload from frontend
        var name = (string)payload.name;
        var description = (string?)payload.description;
        var price = (decimal)payload.price;
        var categoryName = (string)payload.category;

        // Find or create category
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
        if (category == null)
        {
            category = new Category
            {
                Name = categoryName,
                Slug = GenerateSlug(categoryName),
                IsVisible = true
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        // Generate SKU if not provided
        var sku = $"PRD-{DateTime.UtcNow.Ticks}";

        // Get main image URL from media
        string? mainImageUrl = null;
        if (payload.media?.mainImage?.url != null)
        {
            mainImageUrl = (string)payload.media.mainImage.url;
        }

        var product = new Product
        {
            Name = name,
            Description = description,
            Sku = sku,
            Price = price,
            SalePrice = payload.salePrice != null ? (decimal?)payload.salePrice : null,
            PurchaseRate = payload.purchaseRate != null ? (decimal?)payload.purchaseRate : 0,
            Stock = 100, // Default stock
            Status = payload.statusActive == true ? "Active" : "Draft",
            CategoryId = category.Id,
            ImageUrl = mainImageUrl
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Add additional images
        if (payload.media?.thumbnails != null)
        {
            foreach (var thumbnail in payload.media.thumbnails)
            {
                var imageUrl = (string)thumbnail.url;
                if (imageUrl != mainImageUrl) // Don't duplicate main image
                {
                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        Url = imageUrl,
                        AltText = (string?)thumbnail.alt ?? name,
                        IsMain = false,
                        SortOrder = 0
                    };
                    _context.ProductImages.Add(productImage);
                }
            }
            await _context.SaveChangesAsync();
        }

        var result = new
        {
            product.Id,
            product.Name,
            product.Description,
            product.Sku,
            product.Price,
            product.SalePrice,
            product.PurchaseRate,
            product.Stock,
            product.Status,
            product.ImageUrl,
            Category = category.Name,
            CategoryId = product.CategoryId,
            MediaUrls = new List<string> { mainImageUrl ?? "" },
            product.CreatedAt
        };

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] dynamic payload)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        // Update basic fields
        product.Name = (string)payload.name;
        product.Description = (string?)payload.description;
        product.Price = (decimal)payload.price;
        product.SalePrice = payload.salePrice != null ? (decimal?)payload.salePrice : null;
        product.PurchaseRate = payload.purchaseRate != null ? (decimal?)payload.purchaseRate : 0;
        product.Status = payload.statusActive == true ? "Active" : "Draft";
        product.UpdatedAt = DateTime.UtcNow;

        // Update category if changed
        var categoryName = (string)payload.category;
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
        if (category != null)
        {
            product.CategoryId = category.Id;
        }

        // Update main image if provided
        if (payload.media?.mainImage?.url != null)
        {
            product.ImageUrl = (string)payload.media.mainImage.url;
        }

        await _context.SaveChangesAsync();

        var result = new
        {
            product.Id,
            product.Name,
            product.Description,
            product.Sku,
            product.Price,
            product.SalePrice,
            product.PurchaseRate,
            product.Stock,
            product.Status,
            product.ImageUrl,
            Category = category?.Name ?? "",
            CategoryId = product.CategoryId,
            MediaUrls = product.Images.Select(i => i.Url).ToList(),
            product.CreatedAt
        };

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        // Delete associated images from filesystem
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            DeleteImageFile(product.ImageUrl);
        }

        foreach (var image in product.Images)
        {
            DeleteImageFile(image.Url);
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private void DeleteImageFile(string imageUrl)
    {
        try
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "products", fileName);
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
