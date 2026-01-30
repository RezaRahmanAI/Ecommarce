namespace Ecommarce.Api.Controllers;

using Ecommarce.Application.Features.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/categories")]
public class PublicCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all categories (Public - No Auth Required)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var query = new GetCategoriesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
