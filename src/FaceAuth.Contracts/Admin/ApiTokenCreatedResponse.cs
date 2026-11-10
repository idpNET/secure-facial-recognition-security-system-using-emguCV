namespace FaceAuth.Contracts.Admin;

public sealed class ApiTokenCreatedResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = [];
    public DateTimeOffset? ExpiresAt { get; set; }
}
