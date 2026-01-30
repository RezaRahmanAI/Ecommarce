namespace Ecommarce.Application.Services.Implementations;

using Ecommarce.Application.Common.Exceptions;
using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Cart.Commands.AddToCart;
using Ecommarce.Application.Features.Cart.DTOs;
using Ecommarce.Application.Services.Interfaces;
using Ecommarce.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class CartService : ICartService
{
    private readonly IApplicationDbContext _context;

    public CartService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto?> GetCartAsync(string userId)
    {
        var cart = _context.Carts
            .FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);

        if (cart == null)
        {
            return null;
        }

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

    public async Task<CartDto> AddToCartAsync(AddToCartCommand request)
    {
        // Verify product exists and has stock
        var product = _context.Products.FirstOrDefault(p => p.Id == request.ProductId && !p.IsDeleted);
        
        if (product == null)
        {
            throw new NotFoundException(nameof(ProductEntity), request.ProductId);
        }

        if (product.Stock < request.Quantity)
        {
            throw new ValidationException(
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

        await _context.SaveChangesAsync(new CancellationToken());

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
