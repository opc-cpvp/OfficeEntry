using Microsoft.Extensions.Configuration;
using Simple.OData.Client;
using System;
using System.Net;

namespace OfficeEntry.Services.Xrm
{
    public class XrmService
    {
        public string ODataUrl { get; set; }

        public XrmService(string odataUrl)
        {
            ODataUrl = odataUrl;
        }

        public ODataClient GetODataClient()
        {
            var uri = new Uri(ODataUrl);
            var clientSettings = new ODataClientSettings(uri, CredentialCache.DefaultCredentials) { IgnoreUnmappedProperties = true };
            return new ODataClient(clientSettings);
        }
    }
}
