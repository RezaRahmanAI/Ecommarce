namespace Ecommarce.Application.Features.Cart.Commands.AddToCart;

using Ecommarce.Application.Common.Exceptions;
using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Cart.DTOs;
using Ecommarce.Domain.Entities;
using FluentValidation;
using MediatR;

public record AddToCartCommand : IRequest<CartDto>
{
    public required string UserId { get; init; }
    public required int ProductId { get; init; }
    public int Quantity { get; init; } = 1;
}

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID is invalid");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");
    }
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, CartDto>
{
    private readonly IApplicationDbContext _context;

    public AddToCartCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // Verify product exists and has stock
        var product = _context.Products.FirstOrDefault(p => p.Id == request.ProductId && !p.IsDeleted);
        
        if (product == null)
        {
            throw new NotFoundException(nameof(ProductEntity), request.ProductId);
        }

        if (product.Stock < request.Quantity)
        {
            throw new Common.Exceptions.ValidationException(
                new[] { new FluentValidation.Results.ValidationFailure("Quantity", $"Only {product.Stock} items available in stock") });
        }

        // Get or create cart for user
        var cart = _context.Carts.FirstOrDefault(c => c.UserId == request.UserId && !c.IsDeleted);
        
        if (cart == null)
        {
            cart = new CartEntity
            {
                UserId = request.UserId,
                Items = new List<CartItemEntity>()
            };
            _context.AddCart(cart);
        }

        // Check if product already in cart
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        
        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += request.Quantity;
            existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
        }
        else
        {
            // Add new item
            var newItem = new CartItemEntity
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                UnitPrice = product.SalePrice ?? product.BasePrice,
                TotalPrice = (product.SalePrice ?? product.BasePrice) * request.Quantity
            };
            cart.Items.Add(newItem);
        }

        // Update cart total
        cart.TotalAmount = cart.Items.Sum(i => i.TotalPrice);

        await _context.SaveChangesAsync(cancellationToken);

        // Return cart DTO
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            TotalAmount = cart.TotalAmount,
            TotalItems = cart.Items.Sum(i => i.Quantity),
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                ProductImageUrl = i.Product?.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                AvailableStock = i.Product?.Stock ?? 0
            }).ToList()
        };
    }
}
