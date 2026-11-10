using FaceAuth.Application.Abstractions;
using FaceAuth.Contracts.Users;
using FaceAuth.Domain;
using FaceAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaceAuth.Application.Services;

public sealed class UserService
{
    private readonly IFaceAuthDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IPasscodeHasher _passcodeHasher;
    private readonly IFaceRecognitionService _faceRecognition;

    public UserService(
        IFaceAuthDbContext db,
        ITenantContext tenant,
        IPasscodeHasher passcodeHasher,
        IFaceRecognitionService faceRecognition)
    {
        _db = db;
        _tenant = tenant;
        _passcodeHasher = passcodeHasher;
        _faceRecognition = faceRecognition;
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        EnsureScope(ApiScopes.UsersWrite);

        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new ArgumentException("UserName is required.");
        if (string.IsNullOrWhiteSpace(request.Passcode) || request.Passcode.Length < 4)
            throw new ArgumentException("Passcode must be at least 4 characters.");

        var name = request.UserName.Trim();
        var exists = await _db.EndUsers.AnyAsync(u => u.UserName == name, ct);
        if (exists)
            throw new InvalidOperationException("A user with this name already exists for the tenant.");

        var (hash, salt) = _passcodeHasher.Hash(request.Passcode);
        var user = new EndUser
        {
            TenantId = _tenant.TenantId!.Value,
            UserName = name,
            ExternalUserId = string.IsNullOrWhiteSpace(request.ExternalUserId) ? null : request.ExternalUserId.Trim(),
            PasscodeHash = hash,
            PasscodeSalt = salt
        };

        _db.EndUsers.Add(user);
        await _db.SaveChangesAsync(ct);

        return Map(user, 0);
    }

    public async Task<UserResponse> GetAsync(Guid userId, CancellationToken ct = default)
    {
        EnsureTenant();
        EnsureScope(ApiScopes.UsersRead);

        var user = await _db.EndUsers
            .Include(u => u.FaceSamples)
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new KeyNotFoundException("User not found.");

        return Map(user, user.FaceSamples.Count);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken ct = default)
    {
        EnsureTenant();
        EnsureScope(ApiScopes.FacesDelete);

        var user = await _db.EndUsers.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new KeyNotFoundException("User not found.");

        _db.EndUsers.Remove(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<EnrollFaceResponse> EnrollFaceAsync(Guid userId, byte[] imageBytes, CancellationToken ct = default)
    {
        EnsureTenant();
        EnsureScope(ApiScopes.FacesEnroll);

        if (imageBytes.Length == 0)
            throw new ArgumentException("Image is required.");

        var user = await _db.EndUsers.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new KeyNotFoundException("User not found.");

        var embedding = await _faceRecognition.ExtractEmbeddingAsync(imageBytes, ct)
            ?? throw new InvalidOperationException("No face detected in the image.");

        var sample = new FaceSample
        {
            TenantId = _tenant.TenantId!.Value,
            EndUserId = user.Id,
            Embedding = embedding
        };

        _db.FaceSamples.Add(sample);
        await _db.SaveChangesAsync(ct);

        return new EnrollFaceResponse
        {
            FaceSampleId = sample.Id,
            UserId = user.Id
        };
    }

    private void EnsureTenant()
    {
        if (!_tenant.IsResolved)
            throw new UnauthorizedAccessException("Tenant context is required.");
    }

    private void EnsureScope(string scope)
    {
        if (!_tenant.Scopes.Contains(scope, StringComparer.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Missing required scope: {scope}");
    }

    private static UserResponse Map(EndUser user, int faceCount) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        ExternalUserId = user.ExternalUserId,
        FaceSampleCount = faceCount,
        CreatedAt = user.CreatedAt
    };
}
