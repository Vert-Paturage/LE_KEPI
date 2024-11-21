namespace Middleware.TestApp.Interfaces;

public sealed record ErpRegisterReponse
{    
    public string[] RegisteredEndpointKeys { get; set; } = [];
}

public interface IErpMiddleware
{
    Task<ErpRegisterReponse> CallRegisterMethodAsync();
    Task<string> SendActionAsync(string actionKey, Dictionary<string, object> data);
}