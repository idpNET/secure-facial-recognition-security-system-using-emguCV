namespace FaceAuth.Application.Abstractions;

public interface IApiTokenHasher
{
    string Hash(string rawToken);
    bool Verify(string rawToken, string storedHash);
}
