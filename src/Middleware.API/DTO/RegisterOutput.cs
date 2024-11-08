namespace Middleware.API.DTO;

public sealed class RegisterOutput
{
    public string[] RegisteredEndpointKeys { get; }

    public RegisterOutput(string[] endpointKeys)
    {
        RegisteredEndpointKeys = endpointKeys;
    }
}