using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSurvey();

            await builder.Build().RunAsync();
        }
    }

    public static class DependencyInjection
    {
        public static IServiceCollection AddSurvey(this IServiceCollection services)
            => services.AddSingleton<ISurveyInterop, SurveyInterop>();
    }
}