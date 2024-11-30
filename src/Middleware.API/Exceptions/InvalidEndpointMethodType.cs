using Middleware.API.EndpointClient;

namespace Middleware.API.Exceptions;

internal sealed class InvalidEndpointMethodType(AppEndpoint endpoint) :
    ProblemException("Invalid endpoint method type",
        $"Endpoint type '{endpoint.Type}' invalid (Key : {endpoint.Key} - App : {endpoint.App.Key})") { }