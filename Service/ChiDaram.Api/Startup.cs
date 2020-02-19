using System;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using ChiDaram.Api.Classes.Middleware;
using ChiDaram.Api.Classes.Security;
using ChiDaram.Api.Classes.Swagger;
using ChiDaram.Common.Classes;
using ChiDaram.Common.Entity;
using ChiDaram.Data.DataService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ZetaLongPaths;

namespace ChiDaram.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Config

            services.Configure<ConnectionStrings>(Configuration.GetSection(nameof(ConnectionStrings)));
            services.AddSingleton(provider => provider.GetService<IOptions<ConnectionStrings>>().Value);

            services.Configure<SoftwareConfig>(Configuration.GetSection(nameof(SoftwareConfig)));
            services.AddSingleton(provider => provider.GetService<IOptions<SoftwareConfig>>().Value);

            #endregion

            services.AddAutoMapper(typeof(Program));
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddHttpContextAccessor();

            #region Security

            #region JWT Authentication

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Constants.Issuer,
                    ValidAudience = Constants.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.SigningKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.UserPassHeaderAuthorizationPolicyName, policy => policy.Requirements.Add(new UserPassHeaderAuthorizationRequirement()));
            });
            services.AddScoped<IAuthorizationHandler, UserPassHeaderAuthorizationHandler>();

            #endregion

            // SecurityContext
            services.AddScoped(provider =>
            {
                var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
                var userClaim = httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(q => q.Type.Equals(Constants.ClaimUserEntityName, StringComparison.InvariantCulture));
                if (userClaim == null) return new SecurityContextMdsCms();
                return new SecurityContextMdsCms
                {
                    User = JsonConvert.DeserializeObject<User>(userClaim.Value),
                    ConnectionId = httpContextAccessor.HttpContext.Connection.Id
                };
            });

            #endregion

            #region Compression

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            #endregion

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ChiDaram Api",
                    Description = "By: <b>Mohammad Dayyan</b> - 0903-333-9923",
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.DocumentFilter<SwaggerSecurityDocumentFilter>();
                c.OperationFilter<SecurityOperationFilter>();
                c.OperationFilter<RequiredHeaderParameterOperationFilter>();
                c.IncludeXmlComments(ZlpPathHelper.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                c.IncludeXmlComments(ZlpPathHelper.Combine(AppContext.BaseDirectory, "ChiDaram.Common.xml"));
                c.IncludeXmlComments(ZlpPathHelper.Combine(AppContext.BaseDirectory, "ChiDaram.Data.xml"));
            });

            #endregion

            #region BusinessObjects

            services.Scan(scan => scan.FromAssemblyOf<BaseDataService>()
                .AddClasses(c => c.AssignableTo<BaseDataService>())
                .AsSelf().WithScopedLifetime());

            #endregion

            services.AddControllers(options =>
            {
#if DEBUG
                options.CacheProfiles.Add(MemoryCacheKeys.CacheProfileName3Min,
                    new CacheProfile
                    {
                        Duration = 0,
                        Location = ResponseCacheLocation.None,
                        VaryByHeader = "User-Agent",
                        VaryByQueryKeys = new[] { "*" },
                        NoStore = true
                    });
#else
                options.CacheProfiles.Add(MemoryCacheKeys.CacheProfileName3Min,
                    new CacheProfile
                    {
                        Duration = 3 * 60,
                        Location = ResponseCacheLocation.Any,
                        VaryByHeader = "User-Agent",
                        VaryByQueryKeys = new[] { "*" }
                    });
#endif
            }).AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();
            app.UseRouting();
            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseResponseCompression();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseFixArabicCharsMiddleware();

            #region Swagger

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "chidaram-api/{documentName}/chidaram-doc.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "ChiDaram Api";
                c.RoutePrefix = "ChiDaramApi";
                c.SwaggerEndpoint("/chidaram-api/v1/chidaram-doc.json", "ChiDaram Api");
                c.InjectStylesheet("/ApiDocs/chidaram.css");
            });

            #endregion

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
