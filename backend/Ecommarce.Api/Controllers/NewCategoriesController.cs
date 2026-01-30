namespace Ecommarce.Api.Controllers;

using Ecommarce.Application.Features.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var query = new GetCategoriesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
