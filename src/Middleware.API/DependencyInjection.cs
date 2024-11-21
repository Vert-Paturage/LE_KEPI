using Middleware.API.Cache;
using Middleware.API.EndpointClient;
using Middleware.API.Interfaces;

namespace Middleware.API;

public static class DependencyInjection
{
    public static void SetupDependencies(this IServiceCollection services, bool ignoreSsl)
    {
        IHttpClientBuilder httpClientBuilder = services.AddHttpClient("HttpClient");
        if (ignoreSsl)
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                }
            );
        
        services.AddSingleton<IEndpointCache, EndpointCache>();
        services.AddScoped<IEndpointHttpClient, EndpointHttpClient>();
    }
}