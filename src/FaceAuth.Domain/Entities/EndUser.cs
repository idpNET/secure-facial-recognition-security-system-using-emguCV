using FaceAuth.Domain.Common;

namespace FaceAuth.Domain.Entities;

public class EndUser : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? ExternalUserId { get; set; }
    public byte[] PasscodeHash { get; set; } = [];
    public byte[] PasscodeSalt { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Tenant? Tenant { get; set; }
    public ICollection<FaceSample> FaceSamples { get; set; } = new List<FaceSample>();
}
