using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Fabric;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.DataProtection.Repositories;
using SFContacts.UI.Security;
using Microsoft.AspNetCore.DataProtection;
using System;

namespace SFContacts.UI {
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
            services.AddSingleton<IXmlRepository, SFDataprotectionKeyRepository>();

            services
                .AddDataProtection()
                .SetApplicationName("SFContacts.UI");

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });
            services.AddScoped<ISessionService, SessionService>();
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var forwardedHeaderOptions = new ForwardedHeadersOptions {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost
            };
            forwardedHeaderOptions.KnownNetworks.Clear();
            forwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeaderOptions);
            const string XForwardedPathBase = "X-Forwarded-PathBase";
            app.Use((context, next) => {
                if(context.Request.Headers.TryGetValue(XForwardedPathBase, out StringValues pathBase)) {
                    context.Request.PathBase = new PathString(pathBase);
                }
                return next();
            });

            app.UseStaticFiles();

            var basePath = app
                .ApplicationServices
                .GetService<StatelessServiceContext>()?
                .ServiceName
                .AbsolutePath;
            basePath = basePath?.Substring(1);

            var cookieAuthOptions = new CookieAuthenticationOptions() {
                AuthenticationScheme = "CookieAuthentication",
                LoginPath = new PathString(string.Concat("/", basePath, "/Account/Login")),
                AccessDeniedPath = new PathString(string.Concat("/", basePath, "/Account/Unauthorized")),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            };

            app.UseCookieAuthentication(cookieAuthOptions);

            app.UseSession();
            app.Use(async (context, next) => {
                context.Session.SetInt32("session_init", 0);
                await next.Invoke();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "AppGatewayDefault",
                    template: string.Concat(basePath, "/{controller=Home}/{action=Index}/{id?}"));

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
