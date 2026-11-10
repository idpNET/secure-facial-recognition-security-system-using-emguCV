namespace FaceAuth.Contracts.Admin;

public sealed class CreateApiTokenRequest
{
    public string Name { get; set; } = "default";
    public string[]? Scopes { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}
