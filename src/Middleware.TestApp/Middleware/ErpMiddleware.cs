using Microsoft.Extensions.Options;

using Middleware.TestApp.Interfaces;

using Newtonsoft.Json;

using System.Text;

namespace Middleware.TestApp.Middleware;

internal sealed record ErpMiddlewareOptions
{
    public string MiddlewareUrl { get; set; } = string.Empty;
    public string AppKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
}

internal sealed class ErpMiddleware(IOptions<ErpMiddlewareOptions> options, IHttpClientFactory httpClientFactory)
    : IErpMiddleware
{
    private const string REGISTER_ENDPOINT = "/register";
    private const string ACTION_ENDPOINT = "/action";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private readonly string _middlewareUrl = options.Value.MiddlewareUrl;
    private readonly string _appKey = options.Value.AppKey;
    private readonly string _apiUrl = options.Value.ApiUrl;

    public async Task<ErpRegisterReponse> CallRegisterMethodAsync()
    {
        string url = $"{_middlewareUrl}{REGISTER_ENDPOINT}";
        HttpContent content = new StringContent(JsonConvert.SerializeObject(new
        {
            appKey = _appKey,
            url = _apiUrl
        }), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception(responseBody);
        return JsonConvert.DeserializeObject<ErpRegisterReponse>(responseBody)!;
    }

    public async Task<string> SendActionAsync(string actionKey, Dictionary<string, object>? data=null, object? body = null)
    {
        string url = $"{_middlewareUrl}{ACTION_ENDPOINT}";
        HttpContent content = new StringContent(JsonConvert.SerializeObject(new
        {
            Key = actionKey,
            Params = data,
            Body = body
        }), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception(responseBody);
        return responseBody;
    }
}