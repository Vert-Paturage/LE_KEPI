using System.Net;
using System.Text;
using System.Text.Json;
using Middleware.API.DTO;
using Middleware.API.Exceptions;
using Middleware.API.Interfaces;
using Newtonsoft.Json;

namespace Middleware.API.EndpointClient;

public sealed class EndpointHttpClient(IHttpClientFactory clientFactory) : IEndpointHttpClient
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient("HttpClient");

    public async Task<List<AppEndpoint>> CallMeuchMapEndpoint(AppData clientData)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"{clientData.ApiUrl}/meuch_map");

        if (!response.IsSuccessStatusCode)
            throw new MeuchMapCallException(clientData.ApiUrl, response.ReasonPhrase);
        string responseBody = await response.Content.ReadAsStringAsync();
        List<MeuchEndpointInput>? endpoints = JsonConvert.DeserializeObject<List<MeuchEndpointInput>>(responseBody);

        if (endpoints is null)
            throw new InvalidMeuchMapResponseException();

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
                throw new EndpointNotCallableException(endpoint.Key, request.RequestUri?.ToString());

            throw new ProblemException("Exception",responseBody);
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
                                     _ => throw new InvalidEndpointMethodType(endpoint)
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
                throw new RouteValueMissingException(routeParam, endpoint);
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
                throw new QueryParamMissingException(queryParam, endpoint);
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