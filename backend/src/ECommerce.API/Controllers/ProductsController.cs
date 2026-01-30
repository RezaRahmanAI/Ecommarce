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

        return Ok(_mapper.Map<ProductDto>(product));
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
