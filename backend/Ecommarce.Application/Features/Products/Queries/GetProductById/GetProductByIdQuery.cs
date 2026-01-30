namespace Ecommarce.Application.Features.Products.Queries.GetProductById;

using Ecommarce.Application.Common.Exceptions;
using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Products.DTOs;
using MediatR;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = _context.Products
            .FirstOrDefault(p => p.Id == request.Id);

        if (product == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.ProductEntity), request.Id);
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            SubCategory = product.SubCategory,
            Tags = product.Tags,
            Badges = product.Badges,
            Price = product.Price,
            SalePrice = product.SalePrice,
            PurchaseRate = product.PurchaseRate,
            BasePrice = product.BasePrice,
            Gender = product.Gender,
            Featured = product.Featured,
            NewArrival = product.NewArrival,
            Sku = product.Sku,
            Stock = product.Stock,
            Status = product.Status,
            ImageUrl = product.ImageUrl,
            StatusActive = product.StatusActive,
            MediaUrls = product.MediaUrls,

            Ratings = product.Ratings == null ? null : new ProductRatingsDto
            {
                AvgRating = product.Ratings.AvgRating,
                ReviewCount = product.Ratings.ReviewCount,
                RatingBreakdown = product.Ratings.RatingBreakdown?.Select(r => new RatingBreakdownItemDto
                {
                    Rating = r.Rating,
                    Percentage = r.Percentage
                }).ToList() ?? new()
            },

            Images = product.Images == null ? null : new ProductImagesDto
            {
                MainImage = new ProductImageDto
                {
                    Type = product.Images.MainImage.Type,
                    Label = product.Images.MainImage.Label,
                    Url = product.Images.MainImage.Url,
                    Alt = product.Images.MainImage.Alt
                },
                Thumbnails = product.Images.Thumbnails?.Select(i => new ProductImageDto
                {
                    Type = i.Type,
                    Label = i.Label,
                    Url = i.Url,
                    Alt = i.Alt
                }).ToList() ?? new()
            },

            Variants = product.Variants == null ? null : new ProductVariantsDto
            {
                Colors = product.Variants.Colors?.Select(c => new VariantColorDto
                {
                    Name = c.Name,
                    Hex = c.Hex,
                    Selected = c.Selected
                }).ToList() ?? new(),
                Sizes = product.Variants.Sizes?.Select(s => new VariantSizeDto
                {
                    Label = s.Label,
                    Stock = s.Stock,
                    Selected = s.Selected
                }).ToList() ?? new()
            },

            Meta = product.Meta == null ? null : new ProductMetaDto
            {
                FabricAndCare = product.Meta.FabricAndCare,
                ShippingAndReturns = product.Meta.ShippingAndReturns
            },

            RelatedProducts = product.RelatedProducts?.Select(r => new RelatedProductInfoDto
            {
                Id = r.Id,
                Name = r.Name,
                Price = r.Price,
                ImageUrl = r.ImageUrl
            }).ToList()
        };
    }
}
