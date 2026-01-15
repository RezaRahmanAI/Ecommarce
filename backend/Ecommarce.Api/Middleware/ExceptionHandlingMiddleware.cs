using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing request.");

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";

            var (statusCode, title, detail) = ex switch
            {
                ArgumentException argumentException => (
                    StatusCodes.Status400BadRequest,
                    "Invalid request payload.",
                    argumentException.Message),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    "The server encountered an unexpected condition.")
            };

            context.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
