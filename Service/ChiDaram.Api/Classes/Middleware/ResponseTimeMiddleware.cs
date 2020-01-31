using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ChiDaram.Api.Classes.Middleware
{
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _next;
        public ResponseTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;
                httpContext.Response.Headers.Add("X-Response-Time", new[] { $"{stopwatch.ElapsedMilliseconds.ToString()} ms" });
                return Task.CompletedTask;
            }, context);
            return _next.Invoke(context);
        }

    }

    public static class ResponseTimeMiddlewareExtensions
    {
        public static IApplicationBuilder AddResponseTimeHeader(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseTimeMiddleware>();
        }
    }
}