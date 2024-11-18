using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using SeminarIntegration.Models;

namespace SeminarIntegration.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            $"An error occurred while processing your request: {exception.Message}");

        var errorResponse = new AppResponse<object>.ErrorResponse
        {
            Message = exception.Message
        };

        switch (exception)
        {
            case BadHttpRequestException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Title = exception.GetType().Name;
                errorResponse.Path = httpContext.Request.Path;
                break;

            default:
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Title = "Internal Server Error";
                errorResponse.Path = httpContext.Request.Path;
                break;
        }

        httpContext.Response.StatusCode = errorResponse.StatusCode;

        await httpContext
            .Response
            .WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }
}