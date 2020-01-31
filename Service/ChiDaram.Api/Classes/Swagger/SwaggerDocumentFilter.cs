using System;
using ChiDaram.Common.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChiDaram.Api.Classes.Swagger
{
    public class SwaggerSecurityDocumentFilter : IDocumentFilter
    {
        private readonly SoftwareConfig _softwareConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SwaggerSecurityDocumentFilter(IHttpContextAccessor httpContextAccessor, SoftwareConfig softwareConfig)
        {
            _httpContextAccessor = httpContextAccessor;
            _softwareConfig = softwareConfig;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var viewSwaggerPassword = _httpContextAccessor.HttpContext.Request.Headers[Constants.SecurityViewSwaggerPasswordHeaderKeyName];
            if (viewSwaggerPassword != _softwareConfig.ViewSwaggerPassword)
                throw new UnauthorizedAccessException();
        }
    }
}
