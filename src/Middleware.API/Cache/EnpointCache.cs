using System.Runtime.InteropServices.JavaScript;
using Microsoft.Extensions.Options;
using Middleware.API.EndpointClient;
using Middleware.API.Interfaces;
using Newtonsoft.Json;

namespace Middleware.API.Cache;

public sealed record EndpointCacheOptions
{
    public string Path { get; set; } = string.Empty;
}

public sealed class EndpointCache : IEndpointCache
{
    private readonly Dictionary<string, AppEndpoint> _cache = [];
    private readonly string _path;
    private readonly ILogger<EndpointCache> _logger;

    public EndpointCache(IOptions<EndpointCacheOptions> options, ILogger<EndpointCache> logger)
    {
        _path = options.Value.Path;
        _logger = logger;
        _cache = ReadFromDisk();
    }

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

    public Task<AppEndpoint?> GetEndpointAsync(string key)
    {
        _cache.TryGetValue(key, out AppEndpoint? endpoint);
        return Task.FromResult(endpoint);
    }

    public Task<IEnumerable<AppEndpoint>> GetAppEndpointsAsync(string appKey)
    {
        return Task.FromResult(_cache.Values.Where(endpoint => endpoint.App.Key == appKey));
    }

    public Task<IEnumerable<AppEndpoint>> GetEndpointsAsync()
    {
        return Task.FromResult(_cache.Values.Select(endpoint => endpoint));
    }

    public Task<IEnumerable<AppData>> GetRegisteredAppsAsync()
    {
        return Task.FromResult(_cache.Values.Select(endpoint => endpoint.App).Distinct());
    }

    private Dictionary<string,AppEndpoint> ReadFromDisk()
    {
        Dictionary<string, AppEndpoint> meuch = [];
        try
        {
            var value = File.ReadAllText(_path);
            meuch = JsonConvert.DeserializeObject<Dictionary<string, AppEndpoint>>(value)!;
        }
        catch (FileNotFoundException)
        {
             
            _logger.LogWarning("Failed to read endpoint cache");
        }
        
        return meuch;
    }

    public void SaveToDisk()
    {
        var miam = JsonConvert.SerializeObject(_cache);
        File.WriteAllText(_path, miam);
    }
}