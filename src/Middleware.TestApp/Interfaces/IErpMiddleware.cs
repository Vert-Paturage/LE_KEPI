using System.Text.Json;

namespace Middleware.TestApp.Interfaces;

public sealed record ErpRegisterReponse
{
    public string[] Actions { get; set; } = [];
}

public sealed record ErpRegister
{
    public string AppKey { get; }
    public string Url { get; }
    public ErpRegister(string appKey, string url)
    {
        AppKey = appKey;
        Url = url;
    }
}

public sealed record ErpAction
{
    public string Key { get; }
    public Dictionary<string, object>? Params { get; }
    public object? Body { get; }
    public ErpAction(string key, Dictionary<string, object>? @params = null, object? body = null)
    {
        Key = key;
        Params = @params;
        Body = body;
    }
}

public sealed record ErpMeuchEndpoint
{
    public required string Key { get; init; }
    public required string Endpoint { get; init; }
    public required string Description { get; init; }
    public required string Type { get; init; }
    public string? RouteFormat { get; init; }
    public string[]? QueryParams { get; init; }
    public string? Body { get; init; }
}

public interface IErpMiddleware
{
    Task<ErpRegisterReponse> CallRegisterMethodAsync();
    Task<string> SendActionAsync(string actionKey, Dictionary<string, object>? data = null, object? body = null);
}