using FaceAuth.Application.Abstractions;

namespace FaceAuth.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public Guid? ApiTokenId { get; private set; }
    public IReadOnlyList<string> Scopes { get; private set; } = Array.Empty<string>();
    public bool IsResolved => TenantId.HasValue;

    public void Set(Guid tenantId, Guid apiTokenId, IReadOnlyList<string> scopes)
    {
        TenantId = tenantId;
        ApiTokenId = apiTokenId;
        Scopes = scopes;
    }
}
