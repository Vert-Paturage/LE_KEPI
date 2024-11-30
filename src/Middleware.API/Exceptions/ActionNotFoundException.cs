namespace Middleware.API.Exceptions;

internal sealed class ActionNotFoundException(string actionKey) : 
    ProblemException("Action not found", $"Action with key : {actionKey} not found") {}