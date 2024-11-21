using Middleware.API.DTO;

namespace Middleware.API.Objects;

public sealed class AppEndpoint
{
    public AppData App { get; }
    public string Key { get; }
    public string Endpoint { get; }
    public string Description { get; }
    public string Type { get;  }
    public string? Format { get;  }
    public string[]? Param { get; }

    public AppEndpoint(AppData app, MeuchEndpointInput endpoint)
    {
        App = app;
        Key = endpoint.Key;
        Endpoint = endpoint.Endpoint;
        Description = endpoint.Description;
        Type = endpoint.Type;
        Format = endpoint.Format;
        Param = endpoint.Param;
    }

    public AppEndpoint(AppData app, string key, string desc, string type, string? format, string[]? param)
    {
        App = app;
        Key = key;
        Description = desc;
        Type = type;
        Format = format;
        Param = param;
    }

    public override string ToString()
    {
        return $"- Key : {Key}\n- Endpoint : {Endpoint}\n- Description : {Description}\n- Type : {Type}\n- Format : {Format}\n- Param : {Param}";
    }
}