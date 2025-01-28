using Middleware.API.DTO;

namespace Middleware.API.EndpointClient;

public sealed class AppEndpoint
{
    public AppData App { get; }
    public string Key { get; }
    public string Endpoint { get; }
    public string Description { get; }
    public string Type { get;  }
    public string? RouteFormat { get;  }
    public string[]? QueryParams { get; }
    public string Body { get; }
    public string Response { get; }

    public AppEndpoint(AppData app, MeuchEndpointInput endpoint)
    {
        App = app;
        Key = endpoint.Key.ToUpper();
        Endpoint = endpoint.Endpoint;
        Description = endpoint.Description;
        Type = endpoint.Type.ToUpper();
        RouteFormat = endpoint.RouteFormat;
        QueryParams = endpoint.QueryParams;
        Body = endpoint.Body ?? "{}";
        Response = endpoint.Response ?? "{}";
    }
}