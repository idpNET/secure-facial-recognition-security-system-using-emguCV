namespace FaceAuth.Contracts.Auth;

public sealed class VerifyRequest
{
    /// <summary>Base64-encoded image bytes (JPEG/PNG). Prefer multipart upload when possible.</summary>
    public string? ImageBase64 { get; set; }
    public string? Passcode { get; set; }
}
