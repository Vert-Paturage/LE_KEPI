namespace Middleware.API.Exceptions;

[Serializable]
internal class ProblemException(string error, string message) : Exception(message)
{
    public string Error { get; } = error;
}