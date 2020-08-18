using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.Extensions.DependencyInjection;

namespace OfficeEntry.WebApp.Area.Identity
{
    public static class AuthenticationServiceCollectionExtensions
    {
        /// <remarks>
        /// Windows authentication mechanism is only available to IE and Edge.
        ///
        /// Since Safari/Chrome/Firefox don't support authentication over
        /// websockets we are forced to use Cookie Authentication in combination
        /// with Windows (Negociate) Authentication.
        ///
        /// The user is authenticated over the Negociate Microsoft Windows
        /// authentication mechanism, a cookie is created and this cookie is then passed
        /// with the websocket to validate the identity of the user.
        /// <see cref="Controllers.ExternalController"/>
        /// </remarks>
        public static void AddNegotiateWithCookieAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = NegotiateDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddNegotiate(configureOptions =>
                {
                    configureOptions.PersistNtlmCredentials = true;
                    configureOptions.PersistKerberosCredentials = true;
                })
                .AddCookie();

            services.AddScoped<ISurveyInterop, SurveyInterop>();
        }
    }
}
