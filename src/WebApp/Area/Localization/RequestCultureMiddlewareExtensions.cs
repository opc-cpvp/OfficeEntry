using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Area.Localization
{
    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseUrlLocalizationAwareWebSockets(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UrlLocalizationAwareWebSocketsMiddleware>();
        }

        public static IApplicationBuilder UseUrlLocalizationRequestLocalization(this IApplicationBuilder builder)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("fr"),
            };

            /*
             * The three default providers are:
             *      1. QueryStringRequestCultureProvider
             *      2. CookieRequestCultureProvider
             *      3. AcceptLanguageHeaderRequestCultureProvider
             * The 4th one is ours:
             *      4. URL base
             */
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            };

            options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
            {
                var currentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                var segments = context.Request.Path.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length > 1 && segments[0].Length == 2)
                {
                    currentCulture = segments[0];
                }

                var requestCulture = new ProviderCultureResult(currentCulture, currentCulture);

                return await Task.FromResult(requestCulture);
            }));

            return builder
                .UseUrlLocalizationAwareWebSockets()
                .UseRequestLocalization(options);
        }
    }
}
