using Newtonsoft.Json.Linq;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;
using System.Text;
using System.Text.RegularExpressions;

namespace OfficeEntry.Infrastructure.Services.Xrm
{
    public class EmailService : IEmailService
    {
        private const string GuidPattern = @"[0-9A-Fa-f]{8}-(?:[0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}";

        private readonly IHttpClientFactory _httpClientFactory;

        public EmailService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Result> SendEmailAsync(Email email)
        {
            // HttpClient instances can generally be treated as .NET objects not requiring disposal.
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0
            var httpClient = _httpClientFactory.CreateClient(NamedHttpClients.Dynamics365ServiceDesk);

            var emailObject = Entities.email.MapFrom(email);

            // Create an email entity
            using var emailContent = new StringContent(emailObject.ToString(), Encoding.UTF8, "application/json");
            using var createResponse = await httpClient.PostAsync("emails", emailContent);

            if (!createResponse.IsSuccessStatusCode)
            {
                throw new Exception(createResponse.ToString());
            }

            createResponse.Headers.TryGetValues("OData-EntityId", out var entityIds);
            var entityId = entityIds!.First();

            var match = Regex.Match(entityId, GuidPattern);
            var emailId = match.Value;

            // Send the email
            var parametersObject = new JObject { { "IssueSend", true } };
            using var sendEmailContent = new StringContent(parametersObject.ToString(), Encoding.UTF8, "application/json");
            using var sendEmailResponse = await httpClient.PostAsync($"emails({emailId})/Microsoft.Dynamics.CRM.SendEmail", sendEmailContent);

            if (!sendEmailResponse.IsSuccessStatusCode)
            {
                throw new Exception(sendEmailResponse.ToString());
            }

            return Result.Success();
        }
    }
}
