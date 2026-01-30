namespace Ecommarce.Application.Services.Implementations;

using Ecommarce.Application.Common.Exceptions;
using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Products.Commands.CreateProduct;
using Ecommarce.Application.Features.Products.DTOs;
using Ecommarce.Application.Features.Products.Queries.GetProducts;
using Ecommarce.Application.Services.Interfaces;
using Ecommarce.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;

    public ProductService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetProductsAsync(GetProductsQuery request)
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

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = _context.Products
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.ProductEntity), id);
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
                MainImage = product.Images.MainImage == null ? new ProductImageDto() : new ProductImageDto
                {
                    Type = product.Images.MainImage.Type ?? "image",
                    Label = product.Images.MainImage.Label ?? string.Empty,
                    Url = product.Images.MainImage.Url ?? string.Empty,
                    Alt = product.Images.MainImage.Alt ?? string.Empty
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

    public async Task<ProductDto> CreateProductAsync(CreateProductCommand request)
    {
         var product = new ProductEntity
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            SubCategory = request.SubCategory,
            BasePrice = request.BasePrice,
            Price = request.SalePrice ?? request.BasePrice,
            SalePrice = request.SalePrice,
            PurchaseRate = request.PurchaseRate,
            Gender = request.Gender,
            Featured = request.Featured,
            NewArrival = request.NewArrival,
            StatusActive = request.StatusActive,
            Status = request.StatusActive ? "Active" : "Draft",
            Sku = $"SKU-{DateTime.UtcNow.Ticks}",
            Stock = 0,
            Tags = request.Tags,
            Badges = request.Badges
        };

        _context.AddProduct(product);
        await _context.SaveChangesAsync(new CancellationToken());

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            SubCategory = product.SubCategory,
            Price = product.Price,
            SalePrice = product.SalePrice,
            BasePrice = product.BasePrice,
            Sku = product.Sku,
            Status = product.Status,
            Stock = product.Stock,
            Featured = product.Featured,
            Gender = product.Gender,
            Tags = product.Tags,
            Badges = product.Badges
        };
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = _context.Products
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.ProductEntity), id);
        }

        product.IsDeleted = true;
        await _context.SaveChangesAsync(new CancellationToken());

        return true;
    }
}
