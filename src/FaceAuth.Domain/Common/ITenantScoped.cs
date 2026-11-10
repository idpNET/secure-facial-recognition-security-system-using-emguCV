namespace FaceAuth.Domain.Common;

public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
