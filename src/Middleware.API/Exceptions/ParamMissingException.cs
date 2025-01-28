using Middleware.API.EndpointClient;

namespace Middleware.API.Exceptions;

internal sealed class ParamMissingException(string paramName, AppEndpoint endpoint) :
    ProblemException("Query param missing",
        $"Query param '{paramName}' is missing (KEY : {endpoint.Key} - Params : {string.Join(',', endpoint.Params!.Select(v => $"{v.Name}:{v.ValueType}"))})") { }