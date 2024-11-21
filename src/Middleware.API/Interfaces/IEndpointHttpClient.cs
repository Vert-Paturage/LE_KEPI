using Middleware.API.Objects;

namespace Middleware.API.Interfaces;

public interface IEndpointHttpClient
{
    Task<string> CallEndpoint(AppEndpoint endpoint, Dictionary<string, object> data);
}