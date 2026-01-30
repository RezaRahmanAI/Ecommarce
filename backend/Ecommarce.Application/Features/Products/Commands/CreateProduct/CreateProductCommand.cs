namespace Ecommarce.Application.Features.Products.Commands.CreateProduct;

using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Products.DTOs;
using Ecommarce.Domain.Entities;
using FluentValidation;
using MediatR;

public record CreateProductCommand : IRequest<ProductDto>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }
    public string SubCategory { get; init; } = string.Empty;
    public decimal BasePrice { get; init; }
    public decimal? SalePrice { get; init; }
    public decimal PurchaseRate { get; init; }
    public string Gender { get; init; } = string.Empty;
    public bool Featured { get; init; }
    public bool NewArrival { get; init; }
    public bool StatusActive { get; init; }
    public List<string> Tags { get; init; } = new();
    public List<string> Badges { get; init; } = new();
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.PurchaseRate)
            .GreaterThanOrEqualTo(0).WithMessage("Purchase rate must be non-negative");

        RuleFor(x => x.SalePrice)
            .LessThan(x => x.BasePrice)
            .When(x => x.SalePrice.HasValue)
            .WithMessage("Sale price must be less than base price");
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
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
        await _context.SaveChangesAsync(cancellationToken);

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
}
