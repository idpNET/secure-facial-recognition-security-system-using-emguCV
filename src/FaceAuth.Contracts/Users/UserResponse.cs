namespace FaceAuth.Contracts.Users;

public sealed class UserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? ExternalUserId { get; set; }
    public int FaceSampleCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
