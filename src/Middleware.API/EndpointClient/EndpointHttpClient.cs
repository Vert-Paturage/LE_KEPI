using System.Net;
using System.Text;
using Middleware.API.Interfaces;
using Middleware.API.Objects;
using Newtonsoft.Json;

namespace Middleware.API.EndpointClient;

public sealed class EndpointHttpClient : IEndpointHttpClient
{

    private readonly HttpClient _httpClient;

    public EndpointHttpClient(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("HttpClient");
    }
    
    public async Task<string> CallEndpoint(AppEndpoint endpoint, Dictionary<string, object> data)
    {
        HttpRequestMessage request = CreateTypedMessage(endpoint, data);

        //if (request.Method == HttpMethod.Post && bodyData is not null)
          //  request.Content =
            //    new StringContent(JsonConvert.SerializeObject(bodyData), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(
                    $"Cannot find endpoint '{endpoint.Key}' ({endpoint.App.ApiUrl}{endpoint.Endpoint})");
            throw new Exception(responseBody);
        }

        return responseBody;
    }

    private HttpRequestMessage CreateTypedMessage(AppEndpoint endpoint, Dictionary<string, object> data)
    {
        data = LowerDataKeys(data);
        string route = $"{endpoint.App.ApiUrl.TrimEnd('/')}/{endpoint.Endpoint.TrimStart('/')}";

        if (!string.IsNullOrEmpty(endpoint.Format))
        {
            string trimmedRouteValues = endpoint.Format.Trim('/');
            string[] routeParams = trimmedRouteValues.Split('/', StringSplitOptions.TrimEntries);
            route = $"{route.TrimEnd('/')}/{trimmedRouteValues}";
            
            foreach (string routeParam in routeParams)
            {
                if (data.TryGetValue(routeParam.ToLower(), out object? value) == false)
                    throw new Exception(
                        $"Route value '{routeParam}' is missing (KEY : {endpoint.Key} - RouteValue : {endpoint.Format})");
                route = route.Replace(routeParam,  value.ToString());
            }
        }

        if (endpoint.Param is not null && endpoint.Param.Length > 0)
        {
            route = $"{route.TrimEnd('/')}?";
            foreach (string queryParam in endpoint.Param)
            {
                if (data.TryGetValue(queryParam.ToLower(), out object? value) == false)
                    throw new Exception(
                        $"Query param '{queryParam}' is missing (KEY : {endpoint.Key} - QueryParam : {string.Join(',',endpoint.Param.Select(v => v))})");
                route += $"{queryParam}={value}&";
            }

            route = route.TrimEnd('&');
        }

        HttpMethod methodeType;
        switch (endpoint.Type.ToUpper())
        {
            case "GET":
                methodeType = HttpMethod.Get;
                break;
            case "POST":
                methodeType = HttpMethod.Post;
                break;
            case "PATCH":
                methodeType = HttpMethod.Patch;
                break;
            default:
                throw new Exception($"Endpoint type '{endpoint.Type}' invalid ( Key : {endpoint.Key} - App : {endpoint.App.Key})");
        }
        
        return new HttpRequestMessage(methodeType, route);
    }

    private Dictionary<string, object> LowerDataKeys(Dictionary<string, object> data)
    {
        Dictionary<string, object> result = new();
        foreach (KeyValuePair<string,object> valuePair in data)
        {
            result[valuePair.Key.ToLower()] = valuePair.Value;
        }

        return result;
    }
    
    
}