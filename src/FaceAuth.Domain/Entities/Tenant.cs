using FaceAuth.Domain.Enums;

namespace FaceAuth.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ApiToken> ApiTokens { get; set; } = new List<ApiToken>();
    public ICollection<EndUser> EndUsers { get; set; } = new List<EndUser>();
}
