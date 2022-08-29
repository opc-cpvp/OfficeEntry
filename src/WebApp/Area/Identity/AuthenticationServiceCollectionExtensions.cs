using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.ValueObjects;
using System.Collections.Immutable;

namespace OfficeEntry.WebApp.Area.Identity;

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
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.SlidingExpiration = true;

                options.EventsType = typeof(CustomCookieAuthenticationEvents);
            });

        services.AddScoped<CustomCookieAuthenticationEvents>();

        services.AddScoped<ISurveyInterop, SurveyInterop>();
    }
}

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
    private readonly IDomainUserService _domainUserService;

    public CustomCookieAuthenticationEvents(IDomainUserService domainUserService)
    {
        _domainUserService = domainUserService;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userPrincipal = context.Principal;

        // Look for the Teams claim.
        var teams = userPrincipal.Claims
            .Where(x => x.Type is "Team")
            .Where(x => x.Issuer is "OPC")
            .Select(x => x.Value)
            .ToImmutableArray();

        if (!teams.Any())
        {
            return;
        }

        var name = userPrincipal.Identity.Name;

        var adGroups = _domainUserService.GetUserGroupsFor(AdAccount.For(name));

        foreach (var team in teams)
        {
            if (!adGroups.Contains(team))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}