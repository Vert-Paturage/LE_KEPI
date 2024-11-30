using Middleware.API.EndpointClient;

namespace Middleware.API.Exceptions;

internal sealed class QueryParamMissingException(string queryParamName, AppEndpoint endpoint) :
    ProblemException("Query param missing",
        $"Query param '{queryParamName}' is missing (KEY : {endpoint.Key} - QueryParam : {string.Join(',', endpoint.QueryParams!.Select(v => v))})") { }