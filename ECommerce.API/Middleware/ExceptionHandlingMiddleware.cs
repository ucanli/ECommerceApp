using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using ECommerce.Application.Exceptions;

namespace ECommerce.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ExceptionHandlingMiddleware(RequestDelegate next, ProblemDetailsFactory problemDetailsFactory)
        {
            _next = next;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }


        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var problem = new ProblemDetails();
            problem.Instance = httpContext.Request.Path;
            problem.Detail = exception?.Message;

            switch (exception)
            {
                case BadRequestException badRequestException:
                    problem.Status = (int)HttpStatusCode.BadRequest;
                    foreach (var validationResult in badRequestException.Errors)
                    {
                        problem.Extensions.Add(validationResult.Key, validationResult.Value);
                    }
                    break;
                case NotFoundException notFoundException:
                    problem.Status = (int)HttpStatusCode.NotFound;
                    break;
                case ExternalServiceUnavailableException externalServiceUnavailableException:
                    problem.Status = (int)HttpStatusCode.ServiceUnavailable;
                    break;
                case ExternalServiceDataException externalServiceDataException:
                    problem.Status = (int)HttpStatusCode.BadGateway;
                    break;
                default:
                    problem.Status = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            ProblemDetails problemDetails;
            if (_problemDetailsFactory != null)
            {
                problemDetails = _problemDetailsFactory.CreateProblemDetails(httpContext, statusCode: problem.Status);
                problem.Title = problemDetails.Title;
                problem.Type = problemDetails.Type;
            }

            var result = new ObjectResult(problem)
            {
                StatusCode = problem.Status
            };
            var response = JsonSerializer.Serialize(result.Value);
            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = problem.Status.GetValueOrDefault();

            await httpContext.Response.WriteAsync(response);
        }

    }
}
