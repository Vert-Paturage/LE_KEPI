namespace Middleware.API.DTO;

public sealed class RegisterOutput(string[] endpointKeys)
{
    public string[] RegisteredEndpointKeys { get; } = endpointKeys;
}