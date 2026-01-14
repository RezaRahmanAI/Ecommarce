using System.ComponentModel.DataAnnotations;

namespace Ecommarce.Api.Dtos;

public sealed class DashboardSummaryDto
{
    public int WidgetCount { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
}

public sealed class DashboardWidgetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class CreateDashboardWidgetRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Value { get; set; } = string.Empty;
}

public sealed class UpdateDashboardWidgetRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Value { get; set; } = string.Empty;
}
