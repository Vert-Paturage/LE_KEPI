using System.Text.Json;
using Middleware.API.EndpointClient;

namespace Middleware.API.Interfaces;

public interface IEndpointHttpClient
{
    Task<List<AppEndpoint>> CallMeuchMapEndpoint(AppData clientData);
    Task<string> CallEndpoint(AppEndpoint endpoint, Dictionary<string, object> data, JsonElement? body=null);
}