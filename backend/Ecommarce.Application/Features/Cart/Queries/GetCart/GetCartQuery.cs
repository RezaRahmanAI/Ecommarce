namespace Ecommarce.Application.Features.Cart.Queries.GetCart;

using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Cart.DTOs;
using MediatR;

public record GetCartQuery(string UserId) : IRequest<CartDto?>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCartQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto?> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = _context.Carts
            .FirstOrDefault(c => c.UserId == request.UserId && !c.IsDeleted);

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
}
