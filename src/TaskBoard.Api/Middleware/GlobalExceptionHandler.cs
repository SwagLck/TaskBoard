using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Common.Exceptions;

namespace TaskBoard.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken
    )
    {
        var problem = exception switch
        {
            ValidationException ve => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Extensions =
                {
                    ["errors"] = ve.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                }
            },

            NotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = exception.Message
            },

            ForbiddenException => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = exception.Message
            },

            ConflictException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = exception.Message
            },

            UnauthorizedException => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = exception.Message
            },


            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occured"
            }
        };

        if (problem.Status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception");
        
        context.Response.StatusCode = problem.Status!.Value;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}