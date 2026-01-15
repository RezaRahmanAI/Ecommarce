using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/products")]
public sealed class AdminProductsController : ControllerBase
{
    private readonly IAdminCatalogService _catalogService;
    private readonly IImageStorageService _imageStorage;

    public AdminProductsController(IAdminCatalogService catalogService, IImageStorageService imageStorage)
    {
        _catalogService = catalogService;
        _imageStorage = imageStorage;
    }

    [HttpGet("catalog")]
    public IActionResult GetCatalog()
    {
        var products = _catalogService.GetProducts()
            .Select(MapProduct)
            .ToList();
        return Ok(products);
    }

    [HttpGet]
    public IActionResult GetProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] string? statusTab,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (items, total) = _catalogService.FilterProducts(searchTerm ?? string.Empty, category ?? string.Empty, statusTab ?? string.Empty, page, pageSize);
        var mappedItems = items.Select(MapProduct).ToList();
        return Ok(new { items = mappedItems, total });
    }

    [HttpGet("filtered")]
    public IActionResult GetFilteredProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] string? statusTab)
    {
        var items = _catalogService.FilterProducts(searchTerm ?? string.Empty, category ?? string.Empty, statusTab ?? string.Empty)
            .Select(MapProduct)
            .ToList();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetProduct(int id)
    {
        var product = _catalogService.GetProduct(id);
        return product is null ? NotFound() : Ok(MapProduct(product));
    }

    [HttpPost]
    public IActionResult CreateProduct(ProductCreatePayload payload)
    {
        return Ok(MapProduct(_catalogService.CreateProduct(payload)));
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateProduct(int id, ProductUpdatePayload payload)
    {
        var product = _catalogService.UpdateProduct(id, payload);
        return product is null ? NotFound() : Ok(MapProduct(product));
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteProduct(int id)
    {
        return Ok(_catalogService.DeleteProduct(id));
    }

    [HttpPost("media")]
    public async Task<IActionResult> UploadMedia()
    {
        if (!Request.HasFormContentType)
        {
            return BadRequest("Expected multipart form data.");
        }

        var form = await Request.ReadFormAsync();
        var files = form.Files;
        if (files.Count == 0)
        {
            return Ok(Array.Empty<string>());
        }

        var results = new List<string>();
        foreach (var file in files)
        {
            var fileName = await _imageStorage.SaveAsync(file, HttpContext.RequestAborted);
            var url = _imageStorage.BuildPublicUrl(Request, fileName);
            if (!string.IsNullOrWhiteSpace(url))
            {
                results.Add(url);
            }
        }

        return Ok(results);
    }

    [HttpPost("{id:int}/media/remove")]
    public IActionResult RemoveMedia(int id, ProductMediaRemovePayload payload)
    {
        var normalized = _imageStorage.NormalizeImageName(payload.MediaUrl) ?? payload.MediaUrl;
        return Ok(_catalogService.RemoveProductMedia(id, normalized));
    }

    private Product MapProduct(Product product)
    {
        var mappedMediaUrls = product.MediaUrls
            .Select(url => _imageStorage.BuildPublicUrl(Request, url))
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => url!)
            .ToList();

        var mainImage = product.Images.MainImage;
        var mappedMainImage = new ProductImage(
            mainImage.Type,
            mainImage.Label,
            _imageStorage.BuildPublicUrl(Request, mainImage.Url) ?? mainImage.Url,
            mainImage.Alt
        );
        var mappedThumbnails = product.Images.Thumbnails.Select(item => new ProductImage(
            item.Type,
            item.Label,
            _imageStorage.BuildPublicUrl(Request, item.Url) ?? item.Url,
            item.Alt
        )).ToList();
        var mappedImages = new ProductImages(mappedMainImage, mappedThumbnails);

        var mappedVariants = product.InventoryVariants.Select(variant => new ProductVariantEdit(
            variant.Label,
            variant.Price,
            variant.Sku,
            variant.Inventory,
            _imageStorage.BuildPublicUrl(Request, variant.ImageUrl) ?? variant.ImageUrl
        )).ToList();

        var mappedRelated = product.RelatedProducts.Select(related => new RelatedProduct(
            related.Id,
            related.Name,
            related.Price,
            _imageStorage.BuildPublicUrl(Request, related.ImageUrl) ?? related.ImageUrl
        )).ToList();

        return new Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            SubCategory = product.SubCategory,
            Tags = [.. product.Tags],
            Badges = [.. product.Badges],
            Price = product.Price,
            SalePrice = product.SalePrice,
            PurchaseRate = product.PurchaseRate,
            Gender = product.Gender,
            Ratings = product.Ratings,
            Images = mappedImages,
            Variants = product.Variants,
            Meta = product.Meta,
            RelatedProducts = mappedRelated,
            Featured = product.Featured,
            NewArrival = product.NewArrival,
            Sku = product.Sku,
            Stock = product.Stock,
            Status = product.Status,
            ImageUrl = _imageStorage.BuildPublicUrl(Request, product.ImageUrl) ?? product.ImageUrl,
            StatusActive = product.StatusActive,
            MediaUrls = mappedMediaUrls,
            BasePrice = product.BasePrice,
            InventoryVariants = mappedVariants
        };
    }
}
