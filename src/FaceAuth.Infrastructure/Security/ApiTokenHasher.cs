using System.Security.Cryptography;
using System.Text;
using FaceAuth.Application.Abstractions;

namespace FaceAuth.Infrastructure.Security;

public sealed class ApiTokenHasher : IApiTokenHasher
{
    public string Hash(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public bool Verify(string rawToken, string storedHash)
    {
        var computed = Hash(rawToken);
        var a = Encoding.UTF8.GetBytes(computed);
        var b = Encoding.UTF8.GetBytes(storedHash.ToLowerInvariant());
        return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
    }
}
