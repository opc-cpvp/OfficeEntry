using Destructurama;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

namespace OfficeEntry.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var assemblyVersion = typeof(Program).Assembly.GetName().Version.ToString();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Destructure.UsingAttributes()
            .Enrich.WithProperty("Version", assemblyVersion)
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers())
            .Filter.ByExcluding("RequestPath = '/health' and StatusCode = 200")
            .CreateLogger();

        try
        {
            Log.Information("Application starting up");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "The application failed to start correctly.");

            if (!(ex.InnerException is null))
                Log.Fatal(ex.InnerException, "The application failed to start correctly see InnerException");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
