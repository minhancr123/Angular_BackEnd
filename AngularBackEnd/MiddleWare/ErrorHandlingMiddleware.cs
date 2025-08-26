using System.Net;
using System.Text.Json;

namespace AngularBackEnd.MiddleWare
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // cho request đi tiếp
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 nếu không xác định
            var result = JsonSerializer.Serialize(new { message = exception.Message });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            Console.WriteLine($"[Error] {exception.Message}");

            return context.Response.WriteAsync(result);
        }
    }
}
