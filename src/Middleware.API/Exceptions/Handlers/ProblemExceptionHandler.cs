using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Middleware.API.Exceptions.Handlers;

internal class ProblemExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ProblemExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        if (exception is HttpRequestException httpRequestException) {
            problemDetails = new ProblemDetails()
            {
                Status = httpRequestException.StatusCode is not null ? (int)httpRequestException.StatusCode : StatusCodes.Status400BadRequest,
                Title = httpRequestException.Message,
                Detail = httpRequestException.InnerException?.Message,
                Type = "Http Request Exception",
            };
        }
        else if(exception is ProblemException problemException) {
            problemDetails = new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = problemException.Error,
                Detail = problemException.Message,
                Type = "Bad Request",
            };
        }
        else {
            return true;
        }

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