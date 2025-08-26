using System.Diagnostics;

namespace AngularBackEnd.MiddleWare
{
    // Middleware chính
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopWatch = Stopwatch.StartNew();

            try
            {
                // Log request
                Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path}");

                await _next(context);

                stopWatch.Stop();

                // Log response
                Console.WriteLine($"[Response] {context.Response.StatusCode} | {stopWatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                stopWatch.Stop();

                // Log error
                Console.WriteLine($"[Error] {ex.Message} | Path: {context.Request.Path}");

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal Server Error");

                // Log chi tiết stack trace
                Console.WriteLine(ex.ToString());
            }
        }
    }

    // Extension phải nằm ngoài class Middleware
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
