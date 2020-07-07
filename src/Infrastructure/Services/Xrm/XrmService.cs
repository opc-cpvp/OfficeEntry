using Simple.OData.Client;
using System;
using System.Net.Http;

namespace OfficeEntry.Infrastructure.Services.Xrm
{
    public abstract class XrmService : IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private bool disposedValue;

        public XrmService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            HttpClient = _httpClientFactory.CreateClient(NamedHttpClients.Dynamics365ServiceDesk);

            var clientSettings = new ODataClientSettings(HttpClient)
            {
                MetadataDocument = MetadataDocument.Value,
                IgnoreUnmappedProperties = true
            };

            Client = new ODataClient(clientSettings);
        }

        protected HttpClient HttpClient { get; }
        protected ODataClient Client { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    HttpClient?.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                // Set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}