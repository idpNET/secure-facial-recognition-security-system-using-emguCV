using System.Text.Json;

namespace FaceAuth.Api.Middleware;

public sealed class ExceptionMappingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMappingMiddleware> _logger;

    public ExceptionMappingMiddleware(RequestDelegate next, ILogger<ExceptionMappingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, ex.Message),
            InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        if (status >= 500)
            _logger.LogError(ex, "Unhandled exception");
        else
            _logger.LogWarning(ex, "Request failed with {StatusCode}", status);

        if (context.Response.HasStarted)
            throw ex;

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var payload = new
        {
            type = $"https://httpstatuses.com/{status}",
            title,
            status
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
