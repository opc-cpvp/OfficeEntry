using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OfficeEntry.Domain.Contracts;
using OfficeEntry.Services.Xrm;

namespace OfficeEntry.API
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
            services.AddScoped<ILocationService, LocationService>(provider => new LocationService(Configuration["AppSettings:ODataUrl"]));
            services.AddScoped<IAccessRequestService, AccessRequestService>(provider => new AccessRequestService(Configuration["AppSettings:ODataUrl"]));

            services.AddControllers();

            // Register the Swagger services
            services.AddSwaggerDocument(cfg =>
            {
                cfg.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Office Entry API";
                    document.Info.Description = "";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Office of the Privacy Commissioner of Canada",
                        Email = string.Empty,
                        Url = "https://priv.gc.ca"
                    };
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Register the Swagger generator and the Swagger UI middlewares
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
