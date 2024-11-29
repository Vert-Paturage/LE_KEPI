using System.Net;
using System.Text;
using System.Text.Json;
using Middleware.API.DTO;
using Middleware.API.Interfaces;
using Newtonsoft.Json;

namespace Middleware.API.EndpointClient;

public sealed class EndpointHttpClient : IEndpointHttpClient
{
    private readonly HttpClient _httpClient;

    public EndpointHttpClient(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("HttpClient");
    }

    public async Task<List<AppEndpoint>> CallMeuchMapEndpoint(AppData clientData)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"{clientData.ApiUrl}/meuch_map");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error when calling {clientData.ApiUrl}/meuch_map");
        string responseBody = await response.Content.ReadAsStringAsync();
        List<MeuchEndpointInput>? endpoints = JsonConvert.DeserializeObject<List<MeuchEndpointInput>>(responseBody);

        if (endpoints is null)
            throw new Exception($"Invalid Endpoints format on meuch_map route");

        return endpoints.Select(endpoint => new AppEndpoint(clientData, endpoint)).ToList();
    }

    public async Task<string> CallEndpoint(AppEndpoint endpoint, Dictionary<string, object> data, JsonElement? body)
    {
        HttpRequestMessage request = CreateTypedRequest(endpoint, data);
        
        if (body is not null)
            request.Content =
                new StringContent(body.Value.GetRawText(), Encoding.UTF8, "application/json");
        
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) {
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(
                    $"Cannot call endpoint '{endpoint.Key}' ({request.RequestUri})");

            throw new Exception(responseBody);
        }

        return responseBody;
    }

    #region PRIVATE
    private HttpRequestMessage CreateTypedRequest(AppEndpoint endpoint, Dictionary<string, object> data)
    {
        data = LowerDataKeys(data);
        string route = $"{endpoint.App.ApiUrl.TrimEnd('/')}/{endpoint.Endpoint.TrimStart('/')}";

        route = AddRouteValues(route, endpoint, data);
        route = AddQueryParams(route, endpoint, data);

        HttpMethod methodeType = endpoint.Type.ToUpper() switch
                                 {
                                     "GET" => HttpMethod.Get,
                                     "POST" => HttpMethod.Post,
                                     "PATCH" => HttpMethod.Patch,
                                     _ => throw new Exception(
                                         $"Endpoint type '{endpoint.Type}' invalid ( Key : {endpoint.Key} - App : {endpoint.App.Key})")
                                 };

        return new HttpRequestMessage(methodeType, route);
    }

    private string AddRouteValues(string route, AppEndpoint endpoint, Dictionary<string, object> data)
    {
        if(string.IsNullOrEmpty((endpoint.RouteFormat)))
            return route;
        
        string trimmedRouteValues = endpoint.RouteFormat.Trim('/');
        string[] routeParams = trimmedRouteValues.Split('/', StringSplitOptions.TrimEntries);
        route = $"{route.TrimEnd('/')}/{trimmedRouteValues}";

        foreach (string routeParam in routeParams) {
            if (data.TryGetValue(routeParam.ToLower(), out object? value) == false)
                throw new Exception(
                    $"Route value '{routeParam}' is missing (KEY : {endpoint.Key} - RouteValue : {endpoint.RouteFormat})");
            route = route.Replace(routeParam, value.ToString());
        }

        return route;
    }

    private string AddQueryParams(string route, AppEndpoint endpoint, Dictionary<string, object> data)
    {
        if (endpoint.QueryParams is null || endpoint.QueryParams.Length == 0) 
            return route;
        
        route = $"{route.TrimEnd('/')}?";

        foreach (string queryParam in endpoint.QueryParams) {
            if (data.TryGetValue(queryParam.ToLower(), out object? value) == false)
                throw new Exception(
                    $"Query param '{queryParam}' is missing (KEY : {endpoint.Key} - QueryParam : {string.Join(',', endpoint.QueryParams.Select(v => v))})");
            route += $"{queryParam}={value}&";
        }
        return route.TrimEnd('&');
    }

    private Dictionary<string, object> LowerDataKeys(Dictionary<string, object> data)
    {
        Dictionary<string, object> result = new();

        foreach (KeyValuePair<string, object> valuePair in data) {
            result[valuePair.Key.ToLower()] = valuePair.Value;
        }

        return result;
    }
    #endregion
}