using System.Security.Claims;
using System.Text.Encodings.Web;
using FaceAuth.Application.Abstractions;
using FaceAuth.Domain.Enums;
using FaceAuth.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FaceAuth.Infrastructure.Auth;

public static class ApiTokenDefaults
{
    public const string Scheme = "ApiToken";
    public const string HeaderName = "X-Api-Key";
}

public sealed class ApiTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly FaceAuthDbContext _db;
    private readonly IApiTokenHasher _hasher;
    private readonly ITenantContext _tenantContext;

    public ApiTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        FaceAuthDbContext db,
        IApiTokenHasher hasher,
        ITenantContext tenantContext)
        : base(options, logger, encoder)
    {
        _db = db;
        _hasher = hasher;
        _tenantContext = tenantContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var raw = ExtractToken();
        if (string.IsNullOrWhiteSpace(raw))
            return AuthenticateResult.NoResult();

        var prefix = ExtractPrefix(raw);
        if (prefix is null)
            return AuthenticateResult.Fail("Invalid API token format.");

        var token = await _db.ApiTokens
            .IgnoreQueryFilters()
            .Include(t => t.Tenant)
            .FirstOrDefaultAsync(t => t.Prefix == prefix);

        if (token is null || !_hasher.Verify(raw, token.Hash))
            return AuthenticateResult.Fail("Invalid API token.");

        if (token.RevokedAt is not null)
            return AuthenticateResult.Fail("API token has been revoked.");

        if (token.ExpiresAt is not null && token.ExpiresAt <= DateTimeOffset.UtcNow)
            return AuthenticateResult.Fail("API token has expired.");

        if (token.Tenant is null || token.Tenant.Status != TenantStatus.Active)
            return AuthenticateResult.Fail("Tenant is not active.");

        token.LastUsedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();

        var scopes = token.GetScopes();
        _tenantContext.Set(token.TenantId, token.Id, scopes);

        var claims = new List<Claim>
        {
            new("tenant_id", token.TenantId.ToString()),
            new("api_token_id", token.Id.ToString()),
            new(ClaimTypes.Name, token.Name)
        };
        claims.AddRange(scopes.Select(s => new Claim("scope", s)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private string? ExtractToken()
    {
        if (Request.Headers.TryGetValue(ApiTokenDefaults.HeaderName, out var key) &&
            !string.IsNullOrWhiteSpace(key))
        {
            return key.ToString().Trim();
        }

        var auth = Request.Headers.Authorization.ToString();
        if (auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return auth["Bearer ".Length..].Trim();

        return null;
    }

    private static string? ExtractPrefix(string rawToken)
    {
        // fa_live_{prefix}_{rest}
        var parts = rawToken.Split('_', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 4 && parts[0] == "fa" && parts[1] == "live")
            return parts[2];
        return null;
    }
}
