using System.Collections.Generic;
using System.Linq;
using ChiDaram.Common.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChiDaram.Api.Classes.Swagger
{
    public class RequiredHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            var allowAnonymousFilter = context.ApiDescription.ActionDescriptor.EndpointMetadata.FirstOrDefault(t => t is AllowAnonymousAttribute);
            if (allowAnonymousFilter != null) return;

            var authorizeFilter = context.ApiDescription.ActionDescriptor.EndpointMetadata.FirstOrDefault(t => t is AuthorizeAttribute);
            if (authorizeFilter == null) return;

            var userPassAuthorizationPolicy = context.ApiDescription.ActionDescriptor.EndpointMetadata.Select(t => (t as AuthorizeAttribute)?.Policy).FirstOrDefault(q => q == Constants.UserPassHeaderAuthorizationPolicyName);
            if (string.IsNullOrWhiteSpace(userPassAuthorizationPolicy)) return;

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = Constants.SecurityUserHeaderName,
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                },
                Required = true
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = Constants.SecurityPassHeaderName,
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "password"
                },
                Required = true
            });
        }
    }
}
