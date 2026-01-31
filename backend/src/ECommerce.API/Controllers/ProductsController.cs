using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IGenericRepository<Product> _productsRepo;
    private readonly IGenericRepository<Category> _categoryRepo;
    private readonly IMapper _mapper;

    public ProductsController(IGenericRepository<Product> productsRepo, IGenericRepository<Category> categoryRepo, IMapper mapper)
    {
        _productsRepo = productsRepo;
        _categoryRepo = categoryRepo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts(string? sort, int? categoryId, string? searchTerm)
    {
        var spec = new ProductsWithCategoriesSpecification(sort, categoryId, searchTerm);
        var products = await _productsRepo.ListAsync(spec);
        return Ok(_mapper.Map<IReadOnlyList<ProductDto>>(products));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var spec = new ProductsWithCategoriesSpecification(id);
        var product = await _productsRepo.GetEntityWithSpec(spec);

        if (product == null) return NotFound();

        // Deserialize JSON fields
        var tags = !string.IsNullOrEmpty(product.Tags)
            ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(product.Tags) ?? new List<string>()
            : new List<string>();
        var badges = !string.IsNullOrEmpty(product.Badges)
            ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(product.Badges) ?? new List<string>()
            : new List<string>();
        var variants = !string.IsNullOrEmpty(product.Variants)
            ? System.Text.Json.JsonSerializer.Deserialize<ProductVariantsDto>(product.Variants) ?? new ProductVariantsDto()
            : new ProductVariantsDto();
        var meta = !string.IsNullOrEmpty(product.Meta)
            ? System.Text.Json.JsonSerializer.Deserialize<ProductMetaDto>(product.Meta) ?? new ProductMetaDto()
            : new ProductMetaDto();

        // Build images
        var mainImage = new ImageDto
        {
            Url = product.ImageUrl ?? "",
            Alt = product.Name,
            Label = "Main",
            Type = "image"
        };
        var thumbnails = product.Images?.Select(img => new ImageDto
        {
            Url = img.Url,
            Alt = img.AltText ?? product.Name,
            Label = "Gallery",
            Type = "image"
        }).ToList() ?? new List<ImageDto>();

        // Default ratings
        var ratings = new ProductRatingsDto
        {
            AvgRating = 0,
            ReviewCount = 0,
            RatingBreakdown = new List<RatingBreakdownDto>
            {
                new() { Rating = 5, Percentage = 0 },
                new() { Rating = 4, Percentage = 0 },
                new() { Rating = 3, Percentage = 0 },
                new() { Rating = 2, Percentage = 0 },
                new() { Rating = 1, Percentage = 0 }
            }
        };

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            Price = product.Price,
            SalePrice = product.SalePrice,
            PurchaseRate = product.PurchaseRate,
            Stock = product.Stock,
            Status = product.Status,
            ImageUrl = product.ImageUrl,
            Category = product.Category?.Name ?? "",
            CategoryId = product.CategoryId,
            SubCategory = product.SubCategory,
            Gender = product.Gender,
            Tags = tags,
            Badges = badges,
            Featured = product.Featured,
            NewArrival = product.NewArrival,
            Images = new ProductImagesDto
            {
                MainImage = mainImage,
                Thumbnails = thumbnails
            },
            Variants = variants,
            Meta = meta,
            Ratings = ratings,
            RelatedProducts = new List<RelatedProductDto>()
        };

        return Ok(dto);
    }
    
    [HttpGet("featured")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetFeaturedProducts()
    {
        // For simplicity, just return top 4 products. Real implementation might have 'IsFeatured' flag
        var products = await _productsRepo.ListAllAsync();
        return Ok(_mapper.Map<IReadOnlyList<ProductDto>>(products.Take(4)));
    }

    [HttpGet("new-arrivals")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetNewArrivals()
    {
         // For simplicity, return latest.
         var products = await _productsRepo.ListAllAsync();
         return Ok(_mapper.Map<IReadOnlyList<ProductDto>>(products.Take(4)));
    }
}
