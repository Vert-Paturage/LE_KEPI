namespace Middleware.API.Exceptions;

internal sealed class EndpointNotCallableException(string endpointKey, string? requestUri) : 
    ProblemException("Cannot access endpoint", $"Cannot call endpoint '{endpointKey}' ({requestUri})") { }