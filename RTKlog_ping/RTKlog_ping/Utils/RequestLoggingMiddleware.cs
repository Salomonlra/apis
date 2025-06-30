using System.Text;

namespace RTKlog_ping.Utils
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering(); // Allows re-reading the request body
            var request = context.Request;

            using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
            {
                string body = await reader.ReadToEndAsync();

                Console.WriteLine($"Request: {request.Method} {request.Path}");
                Console.WriteLine($"Headers: {string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
                Console.WriteLine($"Body: {body}");
                request.Body.Position = 0; // Reset stream for next middleware
            }

            await _next(context);
        }
    }
}


