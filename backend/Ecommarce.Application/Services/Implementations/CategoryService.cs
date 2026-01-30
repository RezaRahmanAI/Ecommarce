namespace Ecommarce.Application.Services.Implementations;

using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Categories.DTOs;
using Ecommarce.Application.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var categories = _context.Categories
            .Where(c => c.IsVisible)
            .OrderBy(c => c.SortOrder)
            .ToList();

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            ParentId = c.ParentId,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
            IsVisible = c.IsVisible,
            ProductCount = c.ProductCount,
            SortOrder = c.SortOrder
        }).ToList();
    }
}
