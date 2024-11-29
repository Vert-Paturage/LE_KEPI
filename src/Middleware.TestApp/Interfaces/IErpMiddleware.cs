namespace Middleware.TestApp.Interfaces;

public sealed record ErpRegisterReponse
{    
    public string[] Actions { get; set; } = [];
}

public interface IErpMiddleware
{
    Task<ErpRegisterReponse> CallRegisterMethodAsync();
    Task<string> SendActionAsync(string actionKey, Dictionary<string, object>? data=null, object? body=null);
}