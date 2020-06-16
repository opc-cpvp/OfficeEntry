using Microsoft.Extensions.Configuration;
using Simple.OData.Client;
using System;
using System.Net;

namespace OfficeEntry.Services.Xrm
{
    public class XrmService
    {
        private readonly IConfiguration Configuration;

        public XrmService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ODataClient GetODataClient(string url)
        {
            var uri = new Uri(url);
            var clientSettings = new ODataClientSettings(uri, CredentialCache.DefaultCredentials) { IgnoreUnmappedProperties = true };
            return new ODataClient(clientSettings);
        }
    }
}
