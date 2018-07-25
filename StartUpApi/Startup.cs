using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StartUpApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using StartUpApi.Data;
using System;

namespace StartUpApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("StartUpDbConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(cfg =>
            {
                cfg.Password.RequireDigit = true;
                cfg.Password.RequiredLength = 6;
                cfg.Password.RequireNonAlphanumeric = true;
                cfg.Password.RequireUppercase = false;
                
                cfg.SignIn.RequireConfirmedEmail = true;

                cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                cfg.Lockout.MaxFailedAccessAttempts = 5;
                
                cfg.User.RequireUniqueEmail = true;

                // if we are accessing the /api and an unauthorized request is made
                // do not redirect to the login page, but simply return "Unauthorized"
                cfg.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                     {
                         if (ctx.Request.Path.StartsWithSegments("/api"))
                             ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                         return Task.FromResult(0);
                     }
                };
            })
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDbInitializer dbSeeder)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            app.UseIdentity();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppConfiguration:Key").Value)),
                    ValidAudience = Configuration.GetSection("AppConfiguration:AppIssuer").Value,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = Configuration.GetSection("AppConfiguration:AppIssuer").Value
                }
            });

            app.UseMvc();

            dbSeeder.Seed().Wait();
        }
    }
}
