using System.Threading.Tasks;
using ChiDaram.Common.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ChiDaram.Api.Classes.Security
{
    public class UserPassHeaderAuthorizationHandler : AuthorizationHandler<UserPassHeaderAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserPassHeaderAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserPassHeaderAuthorizationRequirement requirement)
        {
            if (!(_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(SecurityConfig)) is SecurityConfig fileServerConfig))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            try
            {
                var username = _httpContextAccessor.HttpContext.Request.Headers[Constants.SecurityUserHeaderName];
                var password = _httpContextAccessor.HttpContext.Request.Headers[Constants.SecurityPassHeaderName];
                if (!fileServerConfig.ValidUsers.TryGetValue(username, out var passFromConfig) || !passFromConfig.Equals(password))
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }
            catch
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class UserPassHeaderAuthorizationRequirement : IAuthorizationRequirement
    {
    }
}