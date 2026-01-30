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
            Ratings = product.Ratings,
            Images = product.Images,
            Variants = product.Variants,
            Meta = product.Meta,
            RelatedProducts = product.RelatedProducts
        };
    }
}
