using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PersonalExpenses.Api.Services
{
    public class ExceptionToProblemDetailsService
    {
        private readonly ILogger<ExceptionToProblemDetailsService> _logger;

        public ExceptionToProblemDetailsService(ILogger<ExceptionToProblemDetailsService> logger)
        {
            _logger = logger;
        }

        public ProblemDetails MapExceptionToProblemDetails(Exception exception, HttpContext context)
        {
            return exception switch
            {
                ValidationException validationException => HandleValidationException(validationException),
                KeyNotFoundException keyNotFoundException => HandleKeyNotFoundException(keyNotFoundException),
                ArgumentNullException argumentNullException => HandleArgumentNullException(argumentNullException),
                ArgumentException argumentException => HandleArgumentException(argumentException),
                _ => HandleGenericException(exception, context)
            };
        }

        private ProblemDetails HandleValidationException(ValidationException exception)
        {
            _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", exception.Errors.Select(e => e.ErrorMessage)));

            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "See the errors property for details.",
                Instance = null
            };

            var errors = exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            problemDetails.Extensions.Add("errors", errors);

            return problemDetails;
        }

        private ProblemDetails HandleKeyNotFoundException(KeyNotFoundException exception)
        {
            _logger.LogWarning("Resource not found: {Message}", exception.Message);

            return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Resource Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = exception.Message,
                Instance = null
            };
        }

        private ProblemDetails HandleArgumentException(ArgumentException exception)
        {
            _logger.LogWarning("Invalid argument: {Message}", exception.Message);

            return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Invalid Argument",
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message,
                Instance = null
            };
        }

        private ProblemDetails HandleArgumentNullException(ArgumentNullException exception)
        {
            _logger.LogWarning("Required argument is null: {ParamName}", exception.ParamName);

            return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Required Argument Is Null",
                Status = StatusCodes.Status400BadRequest,
                Detail = $"The parameter '{exception.ParamName}' is required and cannot be null.",
                Instance = null
            };
        }

        private ProblemDetails HandleGenericException(Exception exception, HttpContext context)
        {
            _logger.LogError(exception, "An unhandled exception occurred");

            return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred. Please try again later.",
                Instance = null
            };
        }
    }
}
