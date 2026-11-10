using FaceAuth.Application.Abstractions;
using FaceAuth.Domain.Common;
using FaceAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaceAuth.Infrastructure.Persistence;

public sealed class FaceAuthDbContext : DbContext, IFaceAuthDbContext
{
    private readonly ITenantContext _tenantContext;

    public FaceAuthDbContext(DbContextOptions<FaceAuthDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ApiToken> ApiTokens => Set<ApiToken>();
    public DbSet<EndUser> EndUsers => Set<EndUser>();
    public DbSet<FaceSample> FaceSamples => Set<FaceSample>();
    public DbSet<UsageEvent> UsageEvents => Set<UsageEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<ApiToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Prefix).HasMaxLength(32).IsRequired();
            e.Property(x => x.Hash).HasMaxLength(128).IsRequired();
            e.Property(x => x.Scopes).HasMaxLength(500).IsRequired();
            e.HasIndex(x => x.Prefix).IsUnique();
            e.HasOne(x => x.Tenant).WithMany(t => t.ApiTokens).HasForeignKey(x => x.TenantId);
            e.HasQueryFilter(x => !_tenantContext.TenantId.HasValue || x.TenantId == _tenantContext.TenantId);
        });

        modelBuilder.Entity<EndUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserName).HasMaxLength(100).IsRequired();
            e.Property(x => x.ExternalUserId).HasMaxLength(200);
            e.Property(x => x.PasscodeHash).IsRequired();
            e.Property(x => x.PasscodeSalt).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.UserName }).IsUnique();
            e.HasOne(x => x.Tenant).WithMany(t => t.EndUsers).HasForeignKey(x => x.TenantId);
            e.HasQueryFilter(x => !_tenantContext.TenantId.HasValue || x.TenantId == _tenantContext.TenantId);
        });

        modelBuilder.Entity<FaceSample>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Embedding).HasColumnType("real[]");
            e.HasOne(x => x.EndUser).WithMany(u => u.FaceSamples).HasForeignKey(x => x.EndUserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !_tenantContext.TenantId.HasValue || x.TenantId == _tenantContext.TenantId);
        });

        modelBuilder.Entity<UsageEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Endpoint).HasMaxLength(300).IsRequired();
            e.Property(x => x.Method).HasMaxLength(16).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.Timestamp });
            e.HasQueryFilter(x => !_tenantContext.TenantId.HasValue || x.TenantId == _tenantContext.TenantId);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ITenantScoped>())
        {
            if (entry.State == EntityState.Added && _tenantContext.TenantId.HasValue)
            {
                entry.Entity.TenantId = _tenantContext.TenantId.Value;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
