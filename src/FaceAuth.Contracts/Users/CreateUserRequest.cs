namespace FaceAuth.Contracts.Users;

public sealed class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Passcode { get; set; } = string.Empty;
    public string? ExternalUserId { get; set; }
}
