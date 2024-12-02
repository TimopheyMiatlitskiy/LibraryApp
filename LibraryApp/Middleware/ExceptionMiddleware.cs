using System.Net;
using System.Text.Json;
using LibraryApp.Exceptions;

namespace LibraryApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Проверяем статус после выполнения следующего Middleware
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    throw new ForbiddenException("Доступ запрещён.");
                }
                else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    throw new UnauthorizedAccessException("Неавторизованный доступ.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки запроса: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }


        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError; // По умолчанию 500
            var errorMessage = "Что-то пошло не так.";

            switch (exception)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorMessage = exception.Message;
                    break;

                case AlreadyExistsException:
                    statusCode = HttpStatusCode.Conflict;
                    errorMessage = exception.Message;
                    break;

                case BadRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = exception.Message;
                    break;

                case UnauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorMessage = exception.Message;
                    break;

                case RequestTimeoutException:
                    statusCode = HttpStatusCode.RequestTimeout;
                    errorMessage = exception.Message;
                    break;

                case ForbiddenException:
                    statusCode = HttpStatusCode.Forbidden;
                    errorMessage = exception.Message;
                    break;

                default:
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = errorMessage,
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
