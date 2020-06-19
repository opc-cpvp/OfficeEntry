using Simple.OData.Client;
using System;
using System.Net.Http;

namespace OfficeEntry.Infrastructure.Services
{
    public abstract class XrmService : IDisposable
    {        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private bool disposedValue;

        public XrmService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient(NamedHttpClients.Dynamics365ServiceDesk);

            var clientSettings = new ODataClientSettings(_httpClient)
            {
                IgnoreUnmappedProperties = true
            };

            Client = new ODataClient(clientSettings);
        }

        protected HttpClient HttpClient => _httpClient;
        protected ODataClient Client { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    _httpClient?.Dispose();
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
