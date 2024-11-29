using Middleware.API.DTO;

namespace Middleware.API.EndpointClient;

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
        Key = endpoint.Key.ToUpper();
        Endpoint = endpoint.Endpoint;
        Description = endpoint.Description;
        Type = endpoint.Type.ToUpper();
        Format = endpoint.Format;
        Param = endpoint.Param;
    }

    public override string ToString()
    {
        string param = "null";
        if (Param is not null)
            param = $"[{string.Join(',',Param?.Select(v=>v))}]";
        return $"- Key : {Key}\n- Endpoint : {Endpoint}\n- Description : {Description}\n- Type : {Type}\n- Format : {Format}\n- Param : {param}";
    }
}