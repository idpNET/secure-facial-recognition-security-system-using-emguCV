using FaceAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaceAuth.Application.Abstractions;

public interface IFaceAuthDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<ApiToken> ApiTokens { get; }
    DbSet<EndUser> EndUsers { get; }
    DbSet<FaceSample> FaceSamples { get; }
    DbSet<UsageEvent> UsageEvents { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
