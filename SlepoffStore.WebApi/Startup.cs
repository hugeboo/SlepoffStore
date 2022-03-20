using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SlepoffStore.Core;
using SlepoffStore.WebApi.Middleware;
using SlepoffStore.WebApi.Services;

namespace SlepoffStore.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbms = Configuration["ApplicationSettings:DBMS"];
            var connectionString = Configuration["ApplicationSettings:ConnectionString"];

            services
               .AddControllers()
               .AddJsonOptions(j =>
                {
                    j.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
                    j.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            if (dbms == "SQLite")
            {
                services
                    .AddScoped<IRepository>(s => new SQLiteRepository(connectionString))
                    .AddScoped<IUserRepository>(s => new SQLiteRepository(connectionString));
            }
            else
            {
                throw new Exception("Invalid DBMS");
            }

            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                                        HttpLoggingFields.RequestBody |
                                        HttpLoggingFields.ResponsePropertiesAndHeaders |
                                        HttpLoggingFields.ResponseBody;
                options.RequestHeaders.Add("SS-UserName");
                options.RequestHeaders.Add("SS-DeviceName");
            });

            services.AddScoped<IUserService, UserService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Slepoff Store API",
                    Description = "A Slepoff Store Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Sergio",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/spboyer"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandlerMiddleware();

            app.UseHttpLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (bool.TryParse(Configuration["ApplicationSettings:UseAuthorization"], out bool useAuth) && useAuth)
            {
                app.UseBasicAuthMiddleware();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Slepoff Store API");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
