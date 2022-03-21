using Simple.OData.Client;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public abstract class XrmService
{
    private readonly IHttpClientFactory _httpClientFactory;    

    public XrmService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

        // HttpClient instances can generally be treated as .NET objects not requiring disposal.
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0
        HttpClient = _httpClientFactory.CreateClient(NamedHttpClients.Dynamics365ServiceDesk);

        var clientSettings = new ODataClientSettings(HttpClient)
        {
            MetadataDocument = MetadataDocument.Value,
            IgnoreUnmappedProperties = true
        };

        Client = new ODataClient(clientSettings);
    }

    protected HttpClient HttpClient { get; }

    protected ODataClient Client { get; }
}
