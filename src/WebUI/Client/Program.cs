using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Globalization;
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

            ////builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            ////builder.Services.AddLocalization();
            builder.Services.AddLocalizationWithoutForceLoad();

            // Get last Culture
            var host = builder.Build();
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
            if (result != null)
            {
                var culture = new CultureInfo(result);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            await host.RunAsync();  
        }
    }

    public static class DependencyInjection
    {
        /// <remarks>
        /// By making the IStringLocalizerFactory be a transient service, no need to reload the application to change the language.
        /// TODO: Figure out what is the impact of this change.
        /// https://github.com/dotnet/aspnetcore/blob/5a0526dfd991419d5bce0d8ea525b50df2e37b04/src/Localization/Localization/src/LocalizationServiceCollectionExtensions.cs
        /// </remarks>
        public static IServiceCollection AddLocalizationWithoutForceLoad(this IServiceCollection services)
            => services
            .AddTransient<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>()
            .AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

        public static IServiceCollection AddSurvey(this IServiceCollection services)
            => services.AddSingleton<ISurveyInterop, SurveyInterop>();
    }
}