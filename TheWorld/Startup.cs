﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Services;
using Microsoft.Extensions.Configuration;
using TheWorld.Models;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorld.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TheWorld
{
    public class Startup
    {

        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                                    .SetBasePath(_env.ContentRootPath)
                                    .AddJsonFile("config.json")
                                    .AddEnvironmentVariables();

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton(_config);

            if (_env.IsDevelopment()) //|| _env.IsEnvironment("SomeCustomTestEnvironmentName"))
            {
                services.AddTransient<IMailService, DebugMailService>();
            }
            else
            {
                // TODO: implement real Mail Service
                services.AddTransient<IMailService, DebugMailService>();
            }

            services.AddDbContext<WorldContext>();

            services.AddIdentity<WorldUser, IdentityRole>(config =>
                {
                    config.User.RequireUniqueEmail = true;
                    config.Password.RequiredLength = 8;
                    config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
                    config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = async ctx =>
                        {
                            if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == StatusCodes.Status200OK)
                            {
                                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            }
                            else
                            {
                                ctx.Response.Redirect(ctx.RedirectUri);
                            }

                            await Task.Yield(); // allows non-async code to satisfy async method signature
                        }
                    };
                })
                .AddEntityFrameworkStores<WorldContext>();

            services.AddScoped<IWorldRepository, WorldRepository>();

            services.AddTransient<WorldContextSeedData>();
            services.AddTransient<GeoCoordsService>();

            services.AddLogging();

            services
                .AddMvc(config =>
                {
                    if (_env.IsProduction())
                        config.Filters.Add(new RequireHttpsAttribute());
                })
                .AddJsonOptions(config => config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, WorldContextSeedData seeder, ILoggerFactory loggerFactory)
        {

            // AutoMapper mappings
            Mapper.Initialize(config =>
            {
                config.CreateMap<TripVM, Trip>().ReverseMap(); // ReverseMap creates a 2-way mapping
                config.CreateMap<StopVM, Stop>().ReverseMap();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug(LogLevel.Information);
            }
            else
            {
                loggerFactory.AddDebug(LogLevel.Error);
                //loggerFactory.AddConsole(LogLevel.Error);
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" });
            });

            seeder
                .EnsureSeedData()
                .Wait();
        }
    }
}
