using FaceAuth.Application.Abstractions;
using FaceAuth.Contracts.Auth;
using FaceAuth.Domain;
using Microsoft.EntityFrameworkCore;

namespace FaceAuth.Application.Services;

public sealed class VerifyService
{
    private readonly IFaceAuthDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IFaceRecognitionService _faceRecognition;
    private readonly IPasscodeHasher _passcodeHasher;
    private readonly double _matchThreshold;

    public VerifyService(
        IFaceAuthDbContext db,
        ITenantContext tenant,
        IFaceRecognitionService faceRecognition,
        IPasscodeHasher passcodeHasher)
    {
        _db = db;
        _tenant = tenant;
        _faceRecognition = faceRecognition;
        _passcodeHasher = passcodeHasher;
        _matchThreshold = 0.45; // ArcFace cosine similarity threshold (tune in production)
    }

    public async Task<VerifyResponse> VerifyAsync(byte[] imageBytes, string? passcode, CancellationToken ct = default)
    {
        if (!_tenant.IsResolved)
            throw new UnauthorizedAccessException("Tenant context is required.");
        if (!_tenant.Scopes.Contains(ApiScopes.FacesVerify, StringComparer.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Missing required scope: {ApiScopes.FacesVerify}");
        if (imageBytes.Length == 0)
            throw new ArgumentException("Image is required.");

        var probe = await _faceRecognition.ExtractEmbeddingAsync(imageBytes, ct);
        if (probe is null)
        {
            return new VerifyResponse { Matched = false };
        }

        var samples = await _db.FaceSamples
            .AsNoTracking()
            .Include(s => s.EndUser)
            .ToListAsync(ct);

        if (samples.Count == 0)
            return new VerifyResponse { Matched = false };

        Guid? bestUserId = null;
        string? bestUserName = null;
        double bestScore = double.MinValue;
        Domain.Entities.EndUser? bestUser = null;

        foreach (var sample in samples)
        {
            if (sample.EndUser is null || sample.Embedding.Length == 0)
                continue;

            var score = _faceRecognition.CosineSimilarity(probe, sample.Embedding);
            if (score > bestScore)
            {
                bestScore = score;
                bestUserId = sample.EndUserId;
                bestUserName = sample.EndUser.UserName;
                bestUser = sample.EndUser;
            }
        }

        if (bestUserId is null || bestScore < _matchThreshold)
        {
            return new VerifyResponse
            {
                Matched = false,
                Score = bestScore == double.MinValue ? null : bestScore
            };
        }

        bool? passcodeValid = null;
        if (!string.IsNullOrEmpty(passcode) && bestUser is not null)
        {
            passcodeValid = _passcodeHasher.Verify(passcode, bestUser.PasscodeHash, bestUser.PasscodeSalt);
        }

        return new VerifyResponse
        {
            Matched = true,
            UserId = bestUserId,
            UserName = bestUserName,
            Score = bestScore,
            PasscodeValid = passcodeValid
        };
    }
}
