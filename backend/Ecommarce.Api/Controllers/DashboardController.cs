using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ecommarce.Api.Data;
using Ecommarce.Api.Dtos;
using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public DashboardController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var widgets = await _dbContext.DashboardWidgets
            .Where(widget => widget.UserId == userId)
            .ToListAsync();

        return Ok(new DashboardSummaryDto
        {
            WidgetCount = widgets.Count,
            LastUpdated = widgets.Count == 0
                ? DateTimeOffset.UtcNow
                : widgets.Max(widget => widget.UpdatedAt)
        });
    }

    [HttpGet("widgets")]
    public async Task<ActionResult<IReadOnlyCollection<DashboardWidgetDto>>> GetWidgets()
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var widgets = await _dbContext.DashboardWidgets
            .Where(widget => widget.UserId == userId)
            .OrderByDescending(widget => widget.UpdatedAt)
            .Select(widget => new DashboardWidgetDto
            {
                Id = widget.Id,
                Title = widget.Title,
                Value = widget.Value,
                UpdatedAt = widget.UpdatedAt
            })
            .ToListAsync();

        return Ok(widgets);
    }

    [HttpPost("widgets")]
    public async Task<ActionResult<DashboardWidgetDto>> CreateWidget(CreateDashboardWidgetRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var widget = new DashboardWidget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Value = request.Value,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.DashboardWidgets.Add(widget);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWidgets), new { id = widget.Id }, new DashboardWidgetDto
        {
            Id = widget.Id,
            Title = widget.Title,
            Value = widget.Value,
            UpdatedAt = widget.UpdatedAt
        });
    }

    [HttpPut("widgets/{id:guid}")]
    public async Task<ActionResult<DashboardWidgetDto>> UpdateWidget(Guid id, UpdateDashboardWidgetRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var widget = await _dbContext.DashboardWidgets
            .FirstOrDefaultAsync(entry => entry.Id == id && entry.UserId == userId);

        if (widget is null)
        {
            return NotFound();
        }

        widget.Title = request.Title;
        widget.Value = request.Value;
        widget.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(new DashboardWidgetDto
        {
            Id = widget.Id,
            Title = widget.Title,
            Value = widget.Value,
            UpdatedAt = widget.UpdatedAt
        });
    }

    [HttpDelete("widgets/{id:guid}")]
    public async Task<IActionResult> DeleteWidget(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var widget = await _dbContext.DashboardWidgets
            .FirstOrDefaultAsync(entry => entry.Id == id && entry.UserId == userId);

        if (widget is null)
        {
            return NotFound();
        }

        _dbContext.DashboardWidgets.Remove(widget);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.Name);
    }
}
