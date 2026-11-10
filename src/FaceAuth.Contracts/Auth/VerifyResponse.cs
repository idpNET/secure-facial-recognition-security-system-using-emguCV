namespace FaceAuth.Contracts.Auth;

public sealed class VerifyResponse
{
    public bool Matched { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public double? Score { get; set; }
    public bool? PasscodeValid { get; set; }
}
