using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ChiDaram.Common.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ChiDaram.Api.Classes.Middleware
{
    public class FixArabicCharsMiddleware
    {
        private readonly RequestDelegate _next;
        public FixArabicCharsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.HasFormContentType)
            {
                await _next(httpContext);
                return;
            }
            var requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync() ?? "";
            if (httpContext.Request.QueryString.HasValue)
                httpContext.Request.QueryString = new QueryString(HttpUtility.UrlDecode(httpContext.Request.QueryString.Value).RemoveArabicChars().ToEnglishNumber());
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody.RemoveArabicChars().ToEnglishNumber()));
            await _next(httpContext);
        }
    }

    public static class FixArabicCharsMiddlewareExtensions
    {
        public static IApplicationBuilder UseFixArabicCharsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FixArabicCharsMiddleware>();
        }
    }
}