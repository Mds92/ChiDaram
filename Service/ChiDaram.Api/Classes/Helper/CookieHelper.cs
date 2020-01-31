using System;
using System.Text;
using ChiDaram.Common.Helper;
using Microsoft.AspNetCore.Http;

namespace ChiDaram.Api.Classes.Helper
{
    public static class CookieHelper
    {
        private const string ClientIdCookieName = "cidcn";
        public static string GetClientIdCookie(HttpContext httpContext)
        {
            var clientId = httpContext.Request.Cookies[ClientIdCookieName];
            try
            {
                if (!string.IsNullOrWhiteSpace(clientId))
                    return Encoding.UTF8.GetString(Convert.FromBase64String(clientId)).DecryptString();
            }
            catch
            {
                // ignored
            }

            clientId = DateTime.Now.Ticks.ToString();
            var encryptedClientId = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId.EncryptString()));
            httpContext.Response.Cookies.Append(ClientIdCookieName, encryptedClientId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(3)
            });
            return clientId;
        }
    }
}
