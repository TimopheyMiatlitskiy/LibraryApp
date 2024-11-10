using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // Передаем запрос дальше по цепочке
            }
            catch (Exception ex)
            {
                if (httpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    _logger.LogError($"Произошла ошибка: {ex.Message}");
                    await HandleValidationExceptionAsync(httpContext, ex); // Обрабатываем ошибки валидации
                }
                else
                {
                    _logger.LogError($"Произошла ошибка: {ex.Message}");
                    await HandleExceptionAsync(httpContext, ex); // Обрабатываем исключение
                }
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Что-то пошло не так.",
                    Detailed = exception.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }

        private static async Task HandleValidationExceptionAsync(HttpContext context, Exception exception)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Ошибка валидации данных.",
                    Details = exception?.Message ?? "Некорректный ввод параметров запроса."
                };
                var jsonResponse = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

    // Extension method to add middleware to the HTTP request pipeline
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
