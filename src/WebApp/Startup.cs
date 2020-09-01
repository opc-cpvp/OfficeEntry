using Blazored.LocalStorage;
using Fluxor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeEntry.Application;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Infrastructure;
using OfficeEntry.WebApp.Area.Identity;
using OfficeEntry.WebApp.Area.Identity.Services;
using OfficeEntry.WebApp.Filters;
using Serilog;

namespace OfficeEntry.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddInfrastructure(Configuration);

            services.AddHttpContextAccessor();

            services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, NameUserIdProvider>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddControllers(option => {
                option.Filters.Add<SerilogLoggingActionFilter>();
            });
            services.AddMvcCore(opts =>
            {
                opts.Filters.Add<SerilogLoggingPageFilter>();
            });
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddBlazoredLocalStorage();

            services.AddHealthChecks();

            services.AddNegotiateWithCookieAuthentication();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddFluxor(options =>
            {
                options
                    .ScanAssemblies(typeof(Program).Assembly)
                    .UseReduxDevTools();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var supportedCultures = new[] { "en-CA", "fr-CA" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapHealthChecks("/Ready").WithDisplayName("Not a health check");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
