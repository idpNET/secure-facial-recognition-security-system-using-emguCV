using FaceAuth.Domain.Common;

namespace FaceAuth.Domain.Entities;

public class FaceSample : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid EndUserId { get; set; }
    public float[] Embedding { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public EndUser? EndUser { get; set; }
}
