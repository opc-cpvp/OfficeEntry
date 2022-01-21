using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Infrastructure.Identity;
using OfficeEntry.Infrastructure.Services;
using OfficeEntry.Infrastructure.Services.Xrm;
using System.Net;
using System.Net.Http.Headers;

namespace OfficeEntry.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTime, DateTimeService>();

        services.AddScoped<IAccessRequestService, AccessRequestService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<ITermsAndConditionsService, TermsAndConditionsService>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IDomainUserService, DomainUserService>(provider =>
            new DomainUserService(configuration.GetValue<string>("Domain"))
        );

        {
            var serviceDeskUri = new Uri(configuration.GetConnectionString("ServiceDesk"));

            services.AddHttpClient(NamedHttpClients.Dynamics365ServiceDesk, x =>
            {
                x.BaseAddress = serviceDeskUri;
                x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                Credentials = new CredentialCache { { serviceDeskUri, "NTLM", CredentialCache.DefaultNetworkCredentials } }
            });

            //if (configuration.GetValue<bool>("RefreshMetadataDocument"))
            //{                   
            //    var xml = new XrmServiceCache(IHttpClientFactory)
            //}

            if (File.Exists(configuration.GetValue<string>("MetadataDocument")))
            {
                MetadataDocument.Value = File.ReadAllText(configuration.GetValue<string>("MetadataDocument"));
            }
        }

        services.AddAuthentication()
            .AddIdentityServerJwt();

        return services;
    }
}
