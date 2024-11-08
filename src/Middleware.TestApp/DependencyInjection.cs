using Middleware.TestApp.Interfaces;
using Middleware.TestApp.Middleware;

namespace Middleware.TestApp;

internal static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, string middlewareUrl)
    {
        services.AddHttpClient();

        services.Configure<ErpMiddlewareOptions>(options => {
            options.MiddlewareUrl = middlewareUrl;
        });
        
        services.AddScoped<IErpMiddleware, ErpMiddleware>();
    }
}