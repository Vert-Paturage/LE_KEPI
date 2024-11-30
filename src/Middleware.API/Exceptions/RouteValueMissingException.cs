using Middleware.API.EndpointClient;

namespace Middleware.API.Exceptions;

internal sealed class RouteValueMissingException(string routeParamName, AppEndpoint endpoint) :
    ProblemException("Route value missing",
        $"Route value '{routeParamName}' is missing (KEY : {endpoint.Key} - RouteValue : {endpoint.RouteFormat})") { }