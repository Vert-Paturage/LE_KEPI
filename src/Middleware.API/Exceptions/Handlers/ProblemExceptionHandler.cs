using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Middleware.API.Exceptions.Handlers;

internal class ProblemExceptionHandler(
    IProblemDetailsService problemDetailsService, 
    ILogger<ProblemExceptionHandler> logger
    ) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProblemException problemException)
            return true;

        ProblemDetails problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = problemException.Error,
            Detail = problemException.Message,
            Type = "Bad Request",
        };
        
        logger.LogError(JsonConvert.SerializeObject(problemDetails));

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
            });
    }
}