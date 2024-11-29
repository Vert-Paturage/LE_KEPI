using Middleware.API.EndpointClient;

namespace Middleware.API.Interfaces;

public interface IEndpointCache
{
    Task AddEndpointAsync(AppEndpoint endpointInput);
    Task AddEndpointsAsync(IEnumerable<AppEndpoint> endpoints);
    Task RemoveEndpointAsync(string key);
    Task RemoveAppEndpointsAsync(string appKey);
    Task<AppEndpoint?> GetEndpointAsync(string key);
    Task<IEnumerable<AppEndpoint>> GetAppEndpointsAsync(string appKey);
    Task<IEnumerable<AppEndpoint>> GetEndpointsAsync();
    Task<IEnumerable<AppData>> GetRegisteredAppsAsync();
}