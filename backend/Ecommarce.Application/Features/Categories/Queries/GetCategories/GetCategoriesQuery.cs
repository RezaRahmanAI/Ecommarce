namespace Ecommarce.Application.Features.Categories.Queries.GetCategories;

using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Categories.DTOs;
using MediatR;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
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
