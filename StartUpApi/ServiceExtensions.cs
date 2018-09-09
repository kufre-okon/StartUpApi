using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using StartUpApi.Data.DbScript;
using StartUpApi.Data.Repository;
using StartUpApi.Data.Repository.Infrastructure;
using StartUpApi.Data.Repository.Interface;
using StartUpApi.Service;
using StartUpApi.Services;
using StartUpApi.Services.Interface;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;

namespace StartUpApi
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IDbScriptProcessor, DbScriptProcessor>();
            services.AddTransient<IAuditTrailManager, AuditTrailManager>();
            services.AddTransient<IImageService, ImageService>();

            /*     Repositories */
            services.AddTransient<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddTransient<IApplicationUserRoleRepository, ApplicationUserRoleRepository>();

            /*     Services     */
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IApplicationRoleService, ApplicationRoleService>();
            services.AddTransient<IAuthTokenService, AuthTokenService>();
            services.AddTransient<ISignInService, SignInService>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
            services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            return services;

        }
        public static IServiceCollection RegisterSwaggerServices(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var pathToDoc = configuration["Swagger:FileName"];
            services.AddSwaggerGen(c =>
            {
                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                // c.AddSecurityRequirement(security);
            });
            services.ConfigureSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "StartUp Rest APIs",
                        Version = "v1",
                        Description = "A startUp project APIs build on ASP.NET core",
                        TermsOfService = "None"
                    }
                 );

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, pathToDoc);
                options.IncludeXmlComments(filePath);
                options.DescribeAllEnumsAsStrings();
            });

            // services.AddScoped<ISearchProvider, SearchProvider>();
            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");

                c.DocumentTitle = "StartUp Documentation";
                c.DocExpansion(DocExpansion.List);
            });

            return app;
        }
    }
}
