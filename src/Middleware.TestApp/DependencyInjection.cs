using Middleware.TestApp.Interfaces;
using Middleware.TestApp.Middleware;

namespace Middleware.TestApp;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, string middlewareUrl)
    {
        services.AddHttpClient();

        services.Configure<ErpMiddlewareOptions>(options => {
            options.MiddlewareUrl = middlewareUrl;
        });
        
        services.AddHttpClient<IErpMiddleware, ErpMiddleware>();
    }
}