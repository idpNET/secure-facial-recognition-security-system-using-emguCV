using System.Security.Cryptography;
using System.Text;
using FaceAuth.Application.Abstractions;

namespace FaceAuth.Infrastructure.Security;

public sealed class PasscodeHasher : IPasscodeHasher
{
    private const int SaltSize = 48;
    private const int HashSize = 128;
    private const int Iterations = 100_000;

    public (byte[] Hash, byte[] Salt) Hash(string passcode)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Pbkdf2(passcode, salt);
        return (hash, salt);
    }

    public bool Verify(string passcode, byte[] hash, byte[] salt)
    {
        var computed = Pbkdf2(passcode, salt);
        return CryptographicOperations.FixedTimeEquals(computed, hash);
    }

    private static byte[] Pbkdf2(string passcode, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(passcode),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);
    }
}
