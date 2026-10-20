using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Exceptions;

namespace PortfolioApi.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        ProblemDetails problem;

        if (exception is AppException appException)
        {
            problem = new ()
            {
                Title = appException.Title,
                Status = appException.StatusCode,
                Detail = appException.Detail
            };
        } else
        {
            logger.LogError(exception, "Unhandled exception");
            problem = new()
            {
                Title = "An unexpected error occurred.",
                Status = 500,
            };
        }

        httpContext.Response.StatusCode = problem.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, options: null, contentType: "applicaton/problem+json", ct);
        return true;
    }
}