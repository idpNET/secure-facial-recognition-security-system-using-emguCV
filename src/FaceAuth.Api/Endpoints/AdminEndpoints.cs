using FaceAuth.Api.Auth;
using FaceAuth.Application.Services;
using FaceAuth.Contracts.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FaceAuth.Api.Endpoints;

public static class AdminEndpoints
{
    public static RouteGroupBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/admin")
            .WithTags("Admin")
            .AddEndpointFilter<AdminApiKeyEndpointFilter>();

        group.MapPost("/tenants", async (CreateTenantRequest request, TenantAdminService service, CancellationToken ct) =>
        {
            var result = await service.CreateTenantAsync(request, ct);
            return Results.Created($"/v1/admin/tenants/{result.Id}", result);
        })
        .WithName("CreateTenant")
        .Produces<TenantResponse>(StatusCodes.Status201Created);

        group.MapPost("/tenants/{tenantId:guid}/tokens", async (
            Guid tenantId,
            CreateApiTokenRequest request,
            TenantAdminService service,
            CancellationToken ct) =>
        {
            var result = await service.CreateTokenAsync(tenantId, request, ct);
            return Results.Created($"/v1/admin/tokens/{result.Id}", result);
        })
        .WithName("CreateApiToken")
        .Produces<ApiTokenCreatedResponse>(StatusCodes.Status201Created);

        group.MapPost("/tokens/{tokenId:guid}/revoke", async (
            Guid tokenId,
            TenantAdminService service,
            CancellationToken ct) =>
        {
            await service.RevokeTokenAsync(tokenId, ct);
            return Results.NoContent();
        })
        .WithName("RevokeApiToken")
        .Produces(StatusCodes.Status204NoContent);

        group.MapGet("/usage", async (
            [FromQuery] Guid tenantId,
            TenantAdminService service,
            CancellationToken ct) =>
        {
            var result = await service.GetUsageAsync(tenantId, ct);
            return Results.Ok(result);
        })
        .WithName("GetUsage")
        .Produces<UsageSummaryResponse>();

        return group;
    }
}

public sealed class AdminApiKeyEndpointFilter : IEndpointFilter
{
    public const string HeaderName = "X-Admin-Key";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var options = context.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<AdminOptions>>().Value;
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return Results.Problem("Admin API key is not configured.", statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided) ||
            !string.Equals(provided.ToString(), options.ApiKey, StringComparison.Ordinal))
        {
            return Results.Problem("Invalid or missing admin API key.", statusCode: StatusCodes.Status401Unauthorized);
        }

        return await next(context);
    }
}
