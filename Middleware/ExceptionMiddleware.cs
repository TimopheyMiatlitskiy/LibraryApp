//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace LibraryApp.Middleware
//{
//    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
//    public class ExceptionMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger _logger;

//        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory logger)
//        {
//            _next = next;
//            _logger = logger.CreateLogger("CustomMiddleware");
//        }

//        public async Task Invoke(HttpContext httpContext)
//        {
//            _logger.LogInformation("Custom Middleware Initiate");
//            _logger.LogError($"Что-то пошло не так: Произошла ошибка на сервере.");
//            httpContext.Response.Headers.Append("x-trns", Guid.NewGuid().ToString());
//            httpContext.Response.ContentType = "application/json";
//            await _next(httpContext);
//            //await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("x-trns"), 0, "x-trns".Length);

//        }
//        //public async Task Invoke(HttpContext httpContext)
//        //{
//        //    if (httpContext.Request.Path.StartsWithSegments("/api"))
//        //    {
//        //        Console.WriteLine("API Request Middleware Invoked");
//        //        try
//        //        {
//        //            await _next(httpContext); // Передаем запрос дальше
//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            _logger.LogError($"Что-то пошло не так: {ex.Message}");

//        //            // Проверяем, начался ли уже ответ
//        //            if (!httpContext.Response.HasStarted)
//        //            {
//        //                httpContext.Response.ContentType = "application/json";
//        //                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

//        //                var response = new
//        //                {
//        //                    StatusCode = httpContext.Response.StatusCode,
//        //                    Message = "Произошла ошибка на сервере.",
//        //                    Detailed = ex.Message
//        //                };

//        //                // Записываем JSON-ответ напрямую в тело ответа
//        //                var jsonResponse = JsonSerializer.Serialize(response);
//        //                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse), 0, jsonResponse.Length);

//        //                // Логируем в консоль статус ответа
//        //                Console.WriteLine($"Response: {httpContext.Response.StatusCode}");
//        //            }
//        //            else
//        //            {
//        //                // Если ответ уже начался, выводим сообщение в консоль
//        //                Console.WriteLine("Response already started.");
//        //            }
//        //        }
//        //    }
//        //    else
//        //    {
//        //        await _next(httpContext); // Пропускаем другие запросы
//        //    }

//        //}

//    }

//    // Extension method used to add the middleware to the HTTP request pipeline.
//    public static class ExceptionMiddlewareExtensions
//    {
//        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<ExceptionMiddleware>();
//        }
//    }
//}
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
            //else
            //{
            //    _logger.LogWarning("Response has already started. Cannot modify headers.");
            //}
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
