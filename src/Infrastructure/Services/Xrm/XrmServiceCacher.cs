using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services.Xrm
{
    internal class XrmServiceCacher : XrmService
    {
        public XrmServiceCacher(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task CacheMetadataDocument(string path)
        {
            string metadataDocument = await Client.GetMetadataDocumentAsync();
            await System.IO.File.WriteAllTextAsync(path, metadataDocument);
        }
    }
}