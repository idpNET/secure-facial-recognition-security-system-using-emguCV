using FaceAuth.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FaceAuth.Infrastructure.Persistence;

public sealed class FaceAuthDbContextFactory : IDesignTimeDbContextFactory<FaceAuthDbContext>
{
    public FaceAuthDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FaceAuthDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=faceauth;Username=faceauth;Password=faceauth")
            .Options;

        return new FaceAuthDbContext(options, new DesignTimeTenantContext());
    }

    private sealed class DesignTimeTenantContext : ITenantContext
    {
        public Guid? TenantId => null;
        public Guid? ApiTokenId => null;
        public IReadOnlyList<string> Scopes => Array.Empty<string>();
        public bool IsResolved => false;
        public void Set(Guid tenantId, Guid apiTokenId, IReadOnlyList<string> scopes) { }
    }
}
