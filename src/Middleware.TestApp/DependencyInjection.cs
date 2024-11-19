using Middleware.TestApp.Interfaces;
using Middleware.TestApp.Middleware;

namespace Middleware.TestApp;

internal static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();

        services.Configure<ErpMiddlewareOptions>(options => {
            options.MiddlewareUrl = configuration["MiddlewareUrl"]!;
            options.AppKey = configuration["AppKey"]!;
            options.ApiUrl = configuration["ApiUrl"]!;
        });
        
        services.AddScoped<IErpMiddleware, ErpMiddleware>();
    }
}