using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace api.Etc;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var ex = exception;
        while (ex is AggregateException or TargetInvocationException)
            ex = ex.InnerException ?? ex;

        ProblemDetails problemDetails;
        int statusCode;

        if (ex is AuthenticationError authError)
        {
            statusCode = StatusCodes.Status401Unauthorized;
            problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = authError.Message,
                Type = "https://httpstatuses.com/401"
            };
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "An unexpected error occurred",
                Detail = ex.Message,
                Type = "https://httpstatuses.com/500"
            };
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}