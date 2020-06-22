using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services.Xrm
{
    public class TermsAndConditionsService : XrmService, ITermsAndConditionsService
    {
        private readonly ContactsService _contactsService;

        public TermsAndConditionsService(IHttpClientFactory httpClientFactory, ContactsService contactService)
            : base(httpClientFactory)
        {
            _contactsService = contactService;
        }

        public async Task<(Result Result, bool IsHealthAndSafetyMeasuresAccepted)> GetHealthAndSafetyMeasuresFor(string username)
        {
            var (result, contact) = await _contactsService.GetContact(username);

            if (!result.Succeeded)
                return (result, default(bool));

            return (Result.Success(), contact.gc_usersettings?.gc_healthsafety.HasValue ?? false);
        }

        public async Task<(Result Result, bool IsPrivacyActStatementAccepted)> GetPrivacyActStatementFor(string username)
        {
            var (result, contact) = await _contactsService.GetContact(username);

            if (!result.Succeeded)
                return (result, default(bool));

            return (Result.Success(), contact.gc_usersettings?.gc_privacystatement.HasValue ?? false);
        }

        public Task SetHealthAndSafetyMeasuresFor(string username, bool isHealthAndSafetyMeasuresAccepted)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Result> SetPrivacyActStatementFor(string username, bool isPrivateActStatementAccepted)
        {
            var (result, contact) = await _contactsService.GetContact(username);

            if (!result.Succeeded)
                return (result);

            var settings = await (contact.gc_usersettings is null ? Create() : Update(contact.gc_usersettings.gc_usersettingsid));

            await Client
                .For<contact>()
                .Key(contact.contactid)
                .Set(new { _gc_usersettings_value = settings.gc_usersettingsid })
                .UpdateEntryAsync();

            return Result.Success();

            async Task<gc_usersettings> Create()
            {
                throw new NotImplementedException();
            }

            async Task<gc_usersettings> Update(Guid id)
            {
                    throw new NotImplementedException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            _contactsService?.Dispose();
            base.Dispose(disposing);
        }
    }
}