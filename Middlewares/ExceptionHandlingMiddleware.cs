using Microsoft.AspNetCore.Mvc;
using PersonalExpenses.Api.Services;
using System.Net;

namespace PersonalExpenses.Api.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, ExceptionToProblemDetailsService exceptionService)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An unhandled exception occurred");
                await HandleExceptionAsync(context, exception, exceptionService);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ExceptionToProblemDetailsService exceptionService)
        {
            context.Response.ContentType = "application/json";

            ProblemDetails problemDetails = exceptionService.MapExceptionToProblemDetails(exception, context);
            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
