namespace Ecommarce.Application.Services.Interfaces;

using Ecommarce.Application.Features.Cart.Commands.AddToCart;
using Ecommarce.Application.Features.Cart.DTOs;

public interface ICartService
{
    Task<CartDto?> GetCartAsync(string userId);
    Task<CartDto> AddToCartAsync(AddToCartCommand command);
}
