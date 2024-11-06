using Microsoft.Extensions.Options;
using Middleware.TestApp.Interfaces;

namespace Middleware.TestApp.Middleware;

internal sealed record ErpMiddlewareOptions
{
    public string MiddlewareUrl { get; set; } = string.Empty;
}

internal sealed class ErpMiddleware(IOptions<ErpMiddlewareOptions> options, IHttpClientFactory httpClientFactory)
    : IErpMiddleware
{
    private const string REGISTER_ENDPOINT = "/register";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private readonly string _middlewareUrl = options.Value.MiddlewareUrl;

    public Task CallRegisterMethodAsync()
    {
        return _httpClient.GetAsync($"{_middlewareUrl}{REGISTER_ENDPOINT}");
    }
}