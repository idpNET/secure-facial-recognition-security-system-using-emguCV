using FaceAuth.Application.Abstractions;
using FaceAuth.Domain.Entities;
using FaceAuth.Infrastructure.Persistence;

namespace FaceAuth.Api.Middleware;

public sealed class UsageTrackingMiddleware
{
    private readonly RequestDelegate _next;

    public UsageTrackingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, FaceAuthDbContext db)
    {
        await _next(context);

        if (!tenantContext.IsResolved)
            return;

        if (!context.Request.Path.StartsWithSegments("/v1"))
            return;

        try
        {
            db.UsageEvents.Add(new UsageEvent
            {
                TenantId = tenantContext.TenantId!.Value,
                ApiTokenId = tenantContext.ApiTokenId,
                Endpoint = context.Request.Path.Value ?? "/",
                Method = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                Timestamp = DateTimeOffset.UtcNow
            });
            await db.SaveChangesAsync(context.RequestAborted);
        }
        catch
        {
            // Never fail the request because metering failed.
        }
    }
}
