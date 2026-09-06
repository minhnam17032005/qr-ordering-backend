using System.Diagnostics;

namespace QROrdering.API.Middleware
{
    public class ResponseTimeMiddleware
    {
        // Middleware xử lý request tiếp theo trong pipeline
        private readonly RequestDelegate _next;

        public ResponseTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Bắt đầu đo thời gian xử lý request
            var stopwatch = Stopwatch.StartNew();

            context.Response.OnStarting(() =>
            {
                // Dừng timer trước khi gửi response
                stopwatch.Stop();

                // Gắn thời gian xử lý vào response header
                context.Response.Headers["X-Response-Time"] =
                    $"{stopwatch.ElapsedMilliseconds} ms";

                return Task.CompletedTask;
            });

            // Chuyển request đến middleware tiếp theo
            await _next(context);
        }
    }
}
