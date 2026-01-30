namespace Ecommarce.Application.Features.Products.Queries.GetProducts;

using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Products.DTOs;
using MediatR;

public record GetProductsQuery : IRequest<List<ProductDto>>
{
    public string? SearchTerm { get; init; }
    public string? Category { get; init; }
    public string? StatusTab { get; init; }
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public bool? IsFeatured { get; init; }
    public bool? IsNewArrival { get; init; }
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var dbQuery = _context.Products.AsQueryable();

        // Search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            dbQuery = dbQuery.Where(p => 
                p.Name.ToLower().Contains(searchLower) ||
                p.Sku.ToLower().Contains(searchLower));
        }

        // Category filter
        if (!string.IsNullOrEmpty(request.Category) && request.Category != "All Categories")
        {
            dbQuery = dbQuery.Where(p => p.Category == request.Category);
        }

        // Status filter
        if (!string.IsNullOrEmpty(request.StatusTab))
        {
            switch (request.StatusTab)
            {
                case "Active":
                    dbQuery = dbQuery.Where(p => p.Status == "Active");
                    break;
                case "Drafts":
                    dbQuery = dbQuery.Where(p => p.Status == "Draft");
                    break;
                case "Archived":
                    dbQuery = dbQuery.Where(p => p.Status == "Archived");
                    break;
            }
        }

        // Featured filter
        if (request.IsFeatured.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.Featured == request.IsFeatured.Value);
        }

        // New Arrival filter
        if (request.IsNewArrival.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.NewArrival == request.IsNewArrival.Value);
        }

        // Pagination
        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            var skip = (request.Page.Value - 1) * request.PageSize.Value;
            dbQuery = dbQuery.Skip(skip).Take(request.PageSize.Value);
        }

        // Execute query
        var products = dbQuery.ToList();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            SubCategory = p.SubCategory,
            Tags = p.Tags,
            Badges = p.Badges,
            Price = p.Price,
            SalePrice = p.SalePrice,
            PurchaseRate = p.PurchaseRate,
            BasePrice = p.BasePrice,
            Gender = p.Gender,
            Featured = p.Featured,
            NewArrival = p.NewArrival,
            Sku = p.Sku,
            Stock = p.Stock,
            Status = p.Status,
            ImageUrl = p.ImageUrl,
            StatusActive = p.StatusActive,
            MediaUrls = p.MediaUrls,
            Ratings = p.Ratings,
            Images = p.Images,
            Variants = p.Variants,
            Meta = p.Meta,
            RelatedProducts = p.RelatedProducts
        }).ToList();
    }
}
