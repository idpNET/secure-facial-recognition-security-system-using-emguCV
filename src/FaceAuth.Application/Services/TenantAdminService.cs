using FaceAuth.Application.Abstractions;
using FaceAuth.Contracts.Admin;
using FaceAuth.Domain;
using FaceAuth.Domain.Entities;
using FaceAuth.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FaceAuth.Application.Services;

public sealed class TenantAdminService
{
    private readonly IFaceAuthDbContext _db;
    private readonly IApiTokenHasher _tokenHasher;

    public TenantAdminService(IFaceAuthDbContext db, IApiTokenHasher tokenHasher)
    {
        _db = db;
        _tokenHasher = tokenHasher;
    }

    public async Task<TenantResponse> CreateTenantAsync(CreateTenantRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Tenant name is required.", nameof(request));

        var tenant = new Tenant
        {
            Name = request.Name.Trim(),
            Status = TenantStatus.Active
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);

        return new TenantResponse
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Status = tenant.Status.ToString(),
            CreatedAt = tenant.CreatedAt
        };
    }

    public async Task<ApiTokenCreatedResponse> CreateTokenAsync(
        Guid tenantId,
        CreateApiTokenRequest request,
        CancellationToken ct = default)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct)
            ?? throw new KeyNotFoundException("Tenant not found.");

        if (tenant.Status != TenantStatus.Active)
            throw new InvalidOperationException("Tenant is not active.");

        var scopes = (request.Scopes is { Length: > 0 } ? request.Scopes : ApiScopes.All)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var rawToken = GenerateRawToken(out var prefix);
        var entity = new ApiToken
        {
            TenantId = tenant.Id,
            Name = string.IsNullOrWhiteSpace(request.Name) ? "default" : request.Name.Trim(),
            Prefix = prefix,
            Hash = _tokenHasher.Hash(rawToken),
            Scopes = string.Join(',', scopes),
            ExpiresAt = request.ExpiresAt
        };

        _db.ApiTokens.Add(entity);
        await _db.SaveChangesAsync(ct);

        return new ApiTokenCreatedResponse
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Prefix = entity.Prefix,
            Token = rawToken,
            Scopes = scopes,
            ExpiresAt = entity.ExpiresAt
        };
    }

    public async Task RevokeTokenAsync(Guid tokenId, CancellationToken ct = default)
    {
        var token = await _db.ApiTokens.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == tokenId, ct)
            ?? throw new KeyNotFoundException("Token not found.");

        if (token.RevokedAt is null)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<UsageSummaryResponse> GetUsageAsync(Guid tenantId, CancellationToken ct = default)
    {
        var exists = await _db.Tenants.AnyAsync(t => t.Id == tenantId, ct);
        if (!exists)
            throw new KeyNotFoundException("Tenant not found.");

        var events = await _db.UsageEvents
            .IgnoreQueryFilters()
            .Where(e => e.TenantId == tenantId)
            .GroupBy(e => e.Endpoint)
            .Select(g => new UsageEndpointCount { Endpoint = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        return new UsageSummaryResponse
        {
            TenantId = tenantId,
            TotalRequests = events.Sum(e => e.Count),
            ByEndpoint = events
        };
    }

    private static string GenerateRawToken(out string prefix)
    {
        var random = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(24)).ToLowerInvariant();
        prefix = random[..8];
        return $"fa_live_{prefix}_{random[8..]}";
    }
}
