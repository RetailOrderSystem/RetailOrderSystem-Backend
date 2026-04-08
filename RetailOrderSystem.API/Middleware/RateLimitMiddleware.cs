using System.Collections.Concurrent;
using System.Net;

namespace RetailOrderSystem.API.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;

        // key = IP, value = request timestamps
        private static readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

        private const int LIMIT = 100; // requests
        private static readonly TimeSpan WINDOW = TimeSpan.FromMinutes(1);

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            var timestamps = _requests.GetOrAdd(ip, _ => new List<DateTime>());

            lock (timestamps)
            {
                timestamps.RemoveAll(t => now - t > WINDOW);

                if (timestamps.Count >= LIMIT)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "application/json";
                }
                else
                {
                    timestamps.Add(now);
                }
            }

            if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
            {
                await context.Response.WriteAsync("{\"message\":\"Too many requests. Please try again later.\"}");
                return;
            }

            await _next(context);
        }
    }
}