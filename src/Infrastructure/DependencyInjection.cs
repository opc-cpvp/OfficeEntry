using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Infrastructure.Identity;
using OfficeEntry.Infrastructure.Services;
using OfficeEntry.Infrastructure.Services.Xrm;
using Simple.OData.Client;
using System.Net;
using System.Net.Http.Headers;

namespace OfficeEntry.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTime, DateTimeService>();

        services.AddScoped<IAccessRequestService, AccessRequestService>();
        services.AddScoped<IBuildingRoleService, BuildingRoleService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IFloorPlanService, FloorPlanService>();
        services.AddScoped<ITermsAndConditionsService, TermsAndConditionsService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserSettingsService, UserSettingsService>();

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

            if (File.Exists(configuration.GetValue<string>("MetadataDocument")))
            {
                MetadataDocument.Value = File.ReadAllText(configuration.GetValue<string>("MetadataDocument"));
            }
        }

        services.AddScoped<IODataClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

            // HttpClient instances can generally be treated as .NET objects not requiring disposal.
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0
            var httpClient = httpClientFactory.CreateClient(NamedHttpClients.Dynamics365ServiceDesk);

            var clientSettings = new ODataClientSettings(httpClient)
            {
                MetadataDocument = MetadataDocument.Value,
                IgnoreUnmappedProperties = true
            };

            var client = new ODataClient(clientSettings);

            return client;
        });

        return services;
    }
}
