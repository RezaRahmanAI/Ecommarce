namespace Ecommarce.Api.Controllers;

using Ecommarce.Application.Features.Cart.Commands.AddToCart;
using Ecommarce.Application.Features.Cart.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly Application.Services.Interfaces.ICartService _cartService;

    public CartController(Application.Services.Interfaces.ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Get current user's cart
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var result = await _cartService.GetCartAsync(userId);
        
        if (result == null)
        {
            return Ok(new { items = new List<object>(), totalAmount = 0, totalItems = 0 });
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Add product to cart
    /// </summary>
    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        
        var command = new AddToCartCommand
        {
            UserId = userId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        var result = await _cartService.AddToCartAsync(command);
        return Ok(result);
    }
}

public record AddToCartRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; } = 1;
}
