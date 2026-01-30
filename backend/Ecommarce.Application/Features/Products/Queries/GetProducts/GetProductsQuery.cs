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
            
            Ratings = p.Ratings == null ? null : new ProductRatingsDto
            {
                AvgRating = p.Ratings.AvgRating,
                ReviewCount = p.Ratings.ReviewCount,
                RatingBreakdown = p.Ratings.RatingBreakdown?.Select(r => new RatingBreakdownItemDto
                {
                    Rating = r.Rating,
                    Percentage = r.Percentage
                }).ToList() ?? new()
            },
            
            Images = p.Images == null ? null : new ProductImagesDto
            {
                MainImage = p.Images.MainImage == null ? new ProductImageDto() : new ProductImageDto
                {
                    Type = p.Images.MainImage.Type ?? "image",
                    Label = p.Images.MainImage.Label ?? string.Empty,
                    Url = p.Images.MainImage.Url ?? string.Empty,
                    Alt = p.Images.MainImage.Alt ?? string.Empty
                },
                Thumbnails = p.Images.Thumbnails?.Select(i => new ProductImageDto
                {
                    Type = i.Type ?? "image",
                    Label = i.Label ?? string.Empty,
                    Url = i.Url ?? string.Empty,
                    Alt = i.Alt ?? string.Empty
                }).ToList() ?? new()
            },
            
            Variants = p.Variants == null ? null : new ProductVariantsDto
            {
                Colors = p.Variants.Colors?.Select(c => new VariantColorDto
                {
                    Name = c.Name ?? string.Empty,
                    Hex = c.Hex ?? string.Empty,
                    Selected = c.Selected
                }).ToList() ?? new(),
                Sizes = p.Variants.Sizes?.Select(s => new VariantSizeDto
                {
                    Label = s.Label ?? string.Empty,
                    Stock = s.Stock,
                    Selected = s.Selected
                }).ToList() ?? new()
            },
            
            Meta = p.Meta == null ? null : new ProductMetaDto
            {
                FabricAndCare = p.Meta.FabricAndCare ?? string.Empty,
                ShippingAndReturns = p.Meta.ShippingAndReturns ?? string.Empty
            },
            
            RelatedProducts = p.RelatedProducts?.Select(r => new RelatedProductInfoDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                Price = r.Price,
                ImageUrl = r.ImageUrl ?? string.Empty
            }).ToList()
        }).ToList();
    }
}
