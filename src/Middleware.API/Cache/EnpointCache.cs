using Microsoft.Extensions.Options;
using Middleware.API.DTO;
using Middleware.API.EndpointClient;
using Middleware.API.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        return Task.FromResult(_cache.Values
            .GroupBy(action => action.App.Key)
            .Select(group => group.First().App));
    }

    private Dictionary<string,AppEndpoint> ReadFromDisk()
    {
        Dictionary<string, AppEndpoint> meuch = [];
        try
        {
            var value = File.ReadAllText(_path);
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters = [new AppEndpointJsonConvert()],
            };
            meuch = JsonConvert.DeserializeObject<Dictionary<string, AppEndpoint>>(value, settings)!;
        }
        catch (FileNotFoundException)
        {
            _logger.LogWarning("Failed to read endpoint cache");
        }
        
        return meuch;
    }

    public void SaveToDisk()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Converters = [new AppEndpointJsonConvert()],
            Formatting = Formatting.Indented
        };
        File.WriteAllText(_path, JsonConvert.SerializeObject(_cache, settings));
    }
    
    private class AppEndpointJsonConvert : JsonConverter<AppEndpoint>
    {
        public override void WriteJson(JsonWriter writer, AppEndpoint? value, JsonSerializer serializer)
        {
            if (value == null) return;
            writer.WriteStartObject();
            writer.WritePropertyName("app_key");
            writer.WriteValue(value.App.Key);
            writer.WritePropertyName("app_url");
            writer.WriteValue(value.App.ApiUrl);
            writer.WritePropertyName("key");
            writer.WriteValue(value.Key);
            writer.WritePropertyName("endpoint");
            writer.WriteValue(value.Endpoint);
            writer.WritePropertyName("description");
            writer.WriteValue(value.Description);
            writer.WritePropertyName("type");
            writer.WriteValue(value.Type);
            writer.WritePropertyName("route_format");
            writer.WriteValue(value.RouteFormat);
            writer.WritePropertyName("query_params");
            writer.WriteValue(JsonConvert.SerializeObject(value.QueryParams));
            writer.WritePropertyName("body");
            writer.WriteValue(value.Body);
            writer.WriteEndObject();
        }

        public override AppEndpoint? ReadJson(JsonReader reader, Type objectType, AppEndpoint? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            MeuchEndpointInput endpoint = new MeuchEndpointInput()
            {
                Key = jsonObject["key"]?.ToString()!,
                Endpoint = jsonObject["endpoint"]?.ToString()!,
                Description = jsonObject["description"]?.ToString()!,
                Type = jsonObject["type"]?.ToString()!,
                RouteFormat = jsonObject["route_format"]?.ToString()!,
                QueryParams = JsonConvert.DeserializeObject<string[]>(jsonObject["query_params"]?.ToString()!),
                Body = jsonObject["body"]?.ToString()!
            };
            
            AppData app = new AppData(jsonObject["app_key"]?.ToString()!, jsonObject["app_url"]?.ToString()!);

            return new AppEndpoint(app, endpoint);
        }
    }
}