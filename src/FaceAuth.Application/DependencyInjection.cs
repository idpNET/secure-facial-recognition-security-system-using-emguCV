using FaceAuth.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FaceAuth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TenantAdminService>();
        services.AddScoped<UserService>();
        services.AddScoped<VerifyService>();
        return services;
    }
}
