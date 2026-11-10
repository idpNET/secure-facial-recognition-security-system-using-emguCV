using FaceAuth.Application.Abstractions;
using FaceAuth.Infrastructure.Auth;
using FaceAuth.Infrastructure.Face;
using FaceAuth.Infrastructure.Persistence;
using FaceAuth.Infrastructure.Security;
using FaceAuth.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FaceAuth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FaceRecognitionOptions>(configuration.GetSection("FaceRecognition"));

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<IPasscodeHasher, PasscodeHasher>();
        services.AddScoped<IApiTokenHasher, ApiTokenHasher>();
        services.AddSingleton<IFaceRecognitionService, FaceRecognitionService>();

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<FaceAuthDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IFaceAuthDbContext>(sp => sp.GetRequiredService<FaceAuthDbContext>());

        services.AddAuthentication(ApiTokenDefaults.Scheme)
            .AddScheme<AuthenticationSchemeOptions, ApiTokenAuthenticationHandler>(
                ApiTokenDefaults.Scheme, _ => { });

        services.AddAuthorization();

        return services;
    }
}
