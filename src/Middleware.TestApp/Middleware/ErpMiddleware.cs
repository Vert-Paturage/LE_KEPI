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

    public async Task<ErpRegisterReponse> CallRegisterMethodAsync(string apiUrl)
    {
        string url = $"{_middlewareUrl}{REGISTER_ENDPOINT}";
        HttpContent content = new StringContent(JsonConvert.SerializeObject(new
        {
            url = apiUrl
        }), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception(responseBody);
        return JsonConvert.DeserializeObject<ErpRegisterReponse>(responseBody)!;
    }
}