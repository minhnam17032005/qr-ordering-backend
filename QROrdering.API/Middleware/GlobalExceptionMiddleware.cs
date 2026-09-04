using Microsoft.Data.SqlClient;
using QROrdering.API.Common;
using QROrdering.Application.Exceptions;

namespace QROrdering.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception ex)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            if (ex is SqlException)
            {
                _logger.LogCritical(
                    ex,
                    "Database error while processing {Method} {Path}.",
                    context.Request.Method,
                    context.Request.Path);
            }
            else
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception while processing {Method} {Path}.",
                    context.Request.Method,
                    context.Request.Path);
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            int statusCode;
            ErrorResponse response;

            switch (ex)
            {
                case BadRequestException badRequestEx:

                    statusCode = StatusCodes.Status400BadRequest;

                    response = new ErrorResponse
                    {
                        StatusCode = statusCode,
                        Code = "BAD_REQUEST",
                        Message = badRequestEx.Message,
                        Errors = badRequestEx.Errors
                    };

                    break;

                case NotFoundException:

                    statusCode = StatusCodes.Status404NotFound;

                    response = new ErrorResponse
                    {
                        StatusCode = statusCode,
                        Code = "NOT_FOUND",
                        Message = ex.Message
                    };

                    break;

                case ConflictException:

                    statusCode = StatusCodes.Status409Conflict;

                    response = new ErrorResponse
                    {
                        StatusCode = statusCode,
                        Code = "CONFLICT",
                        Message = ex.Message
                    };

                    break;

                case UnauthorizedException:

                    statusCode = StatusCodes.Status401Unauthorized;

                    response = new ErrorResponse
                    {
                        StatusCode = statusCode,
                        Code = "UNAUTHORIZED",
                        Message = ex.Message
                    };

                    break;

                case ForbiddenException:

                    statusCode = StatusCodes.Status403Forbidden;

                    response = new ErrorResponse
                    {
                        StatusCode = statusCode,
                        Code = "FORBIDDEN",
                        Message = ex.Message
                    };

                    break;

                case TooManyRequestsException:

                    statusCode = StatusCodes.Status429TooManyRequests;

                    response = new ErrorResponse
                    {
                        StatusCode = statusCode,
                        Code = "TOO_MANY_REQUESTS",
                        Message = ex.Message
                    };

                    break;

                default:

                    statusCode = StatusCodes.Status500InternalServerError;

                    response = new ErrorResponse
                    {
                        StatusCode = statusCode,
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An unexpected error occurred."
                    };

                    break;
            }

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
