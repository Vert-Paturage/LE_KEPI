using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Middleware.API.Cache;
using Middleware.API.EndpointClient;
using Middleware.API.Exceptions.Handlers;
using Middleware.API.Interfaces;
using Serilog;

namespace Middleware.API;

public static class DependencyInjection
{
    public static void SetupDependencies(this IServiceCollection services, IConfiguration config)
    {
        IHttpClientBuilder httpClientBuilder = services.AddHttpClient("HttpClient");
        if (config.GetValue<bool>("IgnoreSSL"))
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                }
            );

        services.Configure<EndpointCacheOptions>(options => { options.Path = config["CacheFile"]!; });

        services.AddSingleton<IEndpointCache, EndpointCache>();
        services.AddScoped<IEndpointHttpClient, EndpointHttpClient>();
    }

    public static void AddProblemHandler(this IServiceCollection services)
    {
        services.AddProblemDetails(options => {
            options.CustomizeProblemDetails = context => {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });

        services.AddExceptionHandler<ProblemExceptionHandler>();
    }

    public static void AddLogging(this WebApplicationBuilder builder, IConfiguration config)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.File(
                config["LogPath"]!,
                rollingInterval: RollingInterval.Hour,
                rollOnFileSizeLimit: true,
                outputTemplate:"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3}] {Username} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
    }
}