namespace FaceAuth.Application.Abstractions;

public interface IPasscodeHasher
{
    (byte[] Hash, byte[] Salt) Hash(string passcode);
    bool Verify(string passcode, byte[] hash, byte[] salt);
}
