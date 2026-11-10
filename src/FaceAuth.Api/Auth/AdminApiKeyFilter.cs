using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FaceAuth.Api.Auth;

public sealed class AdminOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AdminApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public const string HeaderName = "X-Admin-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<AdminOptions>>().Value;
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Title = "Admin API key is not configured.",
                Status = StatusCodes.Status503ServiceUnavailable
            })
            { StatusCode = StatusCodes.Status503ServiceUnavailable };
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided) ||
            !string.Equals(provided.ToString(), options.ApiKey, StringComparison.Ordinal))
        {
            context.Result = new UnauthorizedObjectResult(new ProblemDetails
            {
                Title = "Invalid or missing admin API key.",
                Status = StatusCodes.Status401Unauthorized
            });
            return;
        }

        await next();
    }
}
