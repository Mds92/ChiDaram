using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChiDaram.Api.Classes.Swagger
{
    public class SecurityOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            var allowAnonymousFilter = context.ApiDescription.ActionDescriptor.EndpointMetadata.FirstOrDefault(t => t is AllowAnonymousAttribute);
            if (allowAnonymousFilter != null) return;

            var authorizeFilter = context.ApiDescription.ActionDescriptor.EndpointMetadata.FirstOrDefault(t => t is AuthorizeAttribute);
            if (authorizeFilter == null) return;

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
        }
    }
}
