namespace Middleware.API.Exceptions;

internal sealed class MeuchMapCallException(string url, string? errorMessage) :
    ProblemException("meuch_map call exception",
        $"Error when calling {url}/meuch_map (error message : {errorMessage})") { }