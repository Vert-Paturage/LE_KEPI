namespace Middleware.API.Exceptions;

internal sealed class InvalidMeuchMapResponseException() : 
    ProblemException("Invalid meuch_map response", "Invalid Endpoints format on meuch_map route") { }