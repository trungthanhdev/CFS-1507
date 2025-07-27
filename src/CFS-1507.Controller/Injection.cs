using CFS_1507.Application;
using CFS_1507.Controller.Endpoint;
using CFS_1507.Controller.Middlewares;
using CFS_1507.Infrastructure;
using CTCore.DynamicQuery.OData;

namespace CFS_1507.Injection;

public static class Injection
{
    public static void AddCorsConfig(this IServiceCollection service)
    {
        service.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials();
            }));
    }
    public static void AddInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.InjectApplication();
        services.AddInfrastructure(configuration);
        services.AddScoped<TokenRevalidator>();
        services.AddCorsConfig();
    }
}