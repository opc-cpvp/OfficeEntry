using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Infrastructure.Identity;
using OfficeEntry.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simple.OData.Client;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OfficeEntry.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTime, DateTimeService>();

            services.AddTransient<ILocationService, LocationService>();

            services.AddTransient<IDomainUserService, DomainUserService>(provider =>
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
            }

            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}
