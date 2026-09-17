using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Infrastructure.Exceptions;

namespace Orders.API.Infrastructure.ExceptionHandlers
{
    public class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        { 
            var (statusCode, problemDetail) = exception switch
            {
                FluentValidation.ValidationException validationException => (
                    StatusCodes.Status400BadRequest,
                    new ValidationProblemDetails(
                        validationException.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "One or more validation errors occured."
                    }
                ),

                OrderDomainException domainException => (
                    StatusCodes.Status400BadRequest,
                    new ProblemDetails()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Domain rule violation.",
                        Detail = domainException.Message
                    }
                ),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails()
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Internal server error. Try again later."
                    }
                )
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Internal error occured.");
            }
            else
            {
                logger.LogWarning("Handled exception of type {Type}: {Message}", exception.GetType().Name, exception.Message);
            }

            httpContext.Response.StatusCode = statusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetail,
                Exception = exception
            });
        }
    }
}
