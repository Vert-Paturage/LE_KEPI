using Middleware.API.Interfaces;
using Middleware.API.Objects;

namespace Middleware.API.Cache;

public sealed class EndpointCache : IEndpointCache
{
    private readonly Dictionary<string, AppEndpoint> _cache = [];

    public Task AddEndpointAsync(AppEndpoint endpointInput)
    {
        _cache[endpointInput.Key] = endpointInput;

        return Task.CompletedTask;
    }

    public Task AddEndpointsAsync(IEnumerable<AppEndpoint> endpoints)
    {
        foreach (AppEndpoint endpoint in endpoints) {
            _cache[endpoint.Key] = endpoint;
        }

        return Task.CompletedTask;
    }

    public Task RemoveEndpointAsync(string key)
    {
        _cache.Remove(key);

        return Task.CompletedTask;
    }

    public async Task RemoveAppEndpointsAsync(string appKey)
    {
        foreach (AppEndpoint endpoint in await GetAppEndpointsAsync(appKey)) {
            _cache.Remove(endpoint.Key);
        }
    }

    public Task<AppEndpoint> GetEndpointAsync(string key)
    {
        return Task.FromResult(_cache[key]);
    }

    public Task<IEnumerable<AppEndpoint>> GetAppEndpointsAsync(string appKey)
    {
        return Task.FromResult(_cache.Values.Where(endpoint => endpoint.App.Key == appKey));
    }

    public Task<IEnumerable<AppEndpoint>> GetEndpointsAsync()
    {
        return Task.FromResult(_cache.Values.Select(endpoint => endpoint));
    }
}