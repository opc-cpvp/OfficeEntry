using Fluxor;
using OfficeEntry.Application;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Infrastructure;
using OfficeEntry.WebApp.Area.Identity;
using OfficeEntry.WebApp.Area.Identity.Services;
using OfficeEntry.WebApp.Area.Localization;
using OfficeEntry.WebApp.Filters;
using Serilog;

namespace OfficeEntry.WebApp;

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

        services.AddControllers(option =>
        {
            option.Filters.Add<SerilogLoggingActionFilter>();
        });
        services.AddMvcCore(opts =>
        {
            opts.Filters.Add<SerilogLoggingPageFilter>();
        });
        services.AddRazorPages();
        services.AddServerSideBlazor();

        services.AddHealthChecks()
            .AddCheck<HealthCheck>("health_check");

        services.AddNegotiateWithCookieAuthentication();

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.AddFluxor(options =>
        {
            options.ScanAssemblies(typeof(Program).Assembly);
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

        app.UseUrlLocalizationRequestLocalization();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRequestLogContext();

        app.UseRouting();

        app.UseResponseCaching();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<LogUserNameMiddleware>();
        app.UseSerilogRequestLogging();

        app.UseEndpoints(endpoints =>
        {
            // Liveness probes let us know if the app is alive or dead
            endpoints.MapGet("/liveness", async context =>
            {
                await context.Response.WriteAsync("I'm alive");
            }).AllowAnonymous();

            // Readiness probes are designed to know when the app is ready
            // to serve traffic.
            endpoints.MapHealthChecks("/readiness").AllowAnonymous();

            endpoints.MapDefaultControllerRoute();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
