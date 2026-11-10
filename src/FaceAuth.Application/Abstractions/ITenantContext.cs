namespace FaceAuth.Application.Abstractions;

public interface ITenantContext
{
    Guid? TenantId { get; }
    Guid? ApiTokenId { get; }
    IReadOnlyList<string> Scopes { get; }
    bool IsResolved { get; }
    void Set(Guid tenantId, Guid apiTokenId, IReadOnlyList<string> scopes);
}
