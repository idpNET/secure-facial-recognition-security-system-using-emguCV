using FaceAuth.Application.Services;
using FaceAuth.Contracts.Auth;
using FaceAuth.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace FaceAuth.Api.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1")
            .WithTags("FaceAuth")
            .RequireAuthorization();

        group.MapPost("/users", async (CreateUserRequest request, UserService service, CancellationToken ct) =>
        {
            var result = await service.CreateAsync(request, ct);
            return Results.Created($"/v1/users/{result.Id}", result);
        })
        .WithName("CreateUser")
        .Produces<UserResponse>(StatusCodes.Status201Created)
        .RequireAuthorization();

        group.MapGet("/users/{userId:guid}", async (Guid userId, UserService service, CancellationToken ct) =>
        {
            var result = await service.GetAsync(userId, ct);
            return Results.Ok(result);
        })
        .WithName("GetUser")
        .Produces<UserResponse>();

        group.MapDelete("/users/{userId:guid}", async (Guid userId, UserService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(userId, ct);
            return Results.NoContent();
        })
        .WithName("DeleteUser")
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/users/{userId:guid}/faces", async (
            Guid userId,
            HttpRequest request,
            UserService service,
            CancellationToken ct) =>
        {
            var bytes = await ReadImageAsync(request, ct);
            var result = await service.EnrollFaceAsync(userId, bytes, ct);
            return Results.Created($"/v1/users/{userId}/faces/{result.FaceSampleId}", result);
        })
        .DisableAntiforgery()
        .WithName("EnrollFace")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<EnrollFaceResponse>(StatusCodes.Status201Created);

        group.MapPost("/auth/verify", async (
            HttpRequest request,
            VerifyService service,
            CancellationToken ct) =>
        {
            string? passcode = null;
            byte[] bytes;

            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync(ct);
                passcode = form["passcode"].ToString();
                if (form.Files.Count == 0)
                    return Results.BadRequest(new ProblemDetails { Title = "Image file is required." });
                await using var stream = form.Files[0].OpenReadStream();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms, ct);
                bytes = ms.ToArray();
            }
            else if (request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
            {
                var body = await request.ReadFromJsonAsync<VerifyRequest>(cancellationToken: ct)
                    ?? new VerifyRequest();
                passcode = body.Passcode;
                if (string.IsNullOrWhiteSpace(body.ImageBase64))
                    return Results.BadRequest(new ProblemDetails { Title = "ImageBase64 is required." });
                bytes = Convert.FromBase64String(StripDataUrl(body.ImageBase64));
            }
            else
            {
                using var ms = new MemoryStream();
                await request.Body.CopyToAsync(ms, ct);
                bytes = ms.ToArray();
                if (request.Headers.TryGetValue("X-Passcode", out var headerPass))
                    passcode = headerPass.ToString();
            }

            var result = await service.VerifyAsync(bytes, passcode, ct);
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithName("VerifyFace")
        .Produces<VerifyResponse>();

        return group;
    }

    private static async Task<byte[]> ReadImageAsync(HttpRequest request, CancellationToken ct)
    {
        if (request.HasFormContentType)
        {
            var form = await request.ReadFormAsync(ct);
            if (form.Files.Count == 0)
                throw new ArgumentException("Image file is required.");
            await using var stream = form.Files[0].OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            return ms.ToArray();
        }

        if (request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
        {
            var body = await request.ReadFromJsonAsync<VerifyRequest>(cancellationToken: ct);
            if (string.IsNullOrWhiteSpace(body?.ImageBase64))
                throw new ArgumentException("ImageBase64 is required.");
            return Convert.FromBase64String(StripDataUrl(body.ImageBase64));
        }

        using var raw = new MemoryStream();
        await request.Body.CopyToAsync(raw, ct);
        return raw.ToArray();
    }

    private static string StripDataUrl(string value)
    {
        var idx = value.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
        return idx >= 0 ? value[(idx + 7)..] : value;
    }
}
