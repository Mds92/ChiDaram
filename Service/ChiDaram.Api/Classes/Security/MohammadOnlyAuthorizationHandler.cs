using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ChiDaram.Api.Classes.Security
{
    public class MohammadOnlyAuthorizationHandler : AuthorizationHandler<MohammadOnlyAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MohammadOnlyAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MohammadOnlyAuthorizationRequirement requirement)
        {
            try
            {
                var username = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (string.IsNullOrWhiteSpace(username) || !username.Equals("Mohammad", StringComparison.InvariantCulture))
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

    public class MohammadOnlyAuthorizationRequirement : IAuthorizationRequirement
    {
    }
}
