using Microsoft.Extensions.Options;

using Middleware.TestApp.Interfaces;

using Newtonsoft.Json;

using System.Text;

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

    public async Task<ErpRegisterReponse> CallRegisterMethodAsync(bool useSecureHttp, int apiPort)
    {
        string url = $"{_middlewareUrl}{REGISTER_ENDPOINT}";
        HttpContent content = new StringContent(JsonConvert.SerializeObject(new
        {
            secureHttp = useSecureHttp,
            port = 7204
        }), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        if(!response.IsSuccessStatusCode)
            throw new Exception(responseBody);
        return JsonConvert.DeserializeObject<ErpRegisterReponse>(responseBody)!;
    }
}