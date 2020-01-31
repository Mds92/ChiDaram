using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ChiDaram.Api.Classes.Security
{
    public class CheckRefererAuthorizationHandler : AuthorizationHandler<CheckRefererAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CheckRefererAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckRefererAuthorizationRequirement requirement)
        {
            try
            {
                var referer = new Uri(_httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString()).Host;
                if (!referer.Equals("olgoirani.com", StringComparison.InvariantCultureIgnoreCase) &&
                    !referer.Equals("files.olgoirani.com", StringComparison.InvariantCultureIgnoreCase) &&
                    !referer.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }
            catch
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Fail();
                return Task.CompletedTask;
            }
            
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class CheckRefererAuthorizationRequirement : IAuthorizationRequirement
    {
    }
}
