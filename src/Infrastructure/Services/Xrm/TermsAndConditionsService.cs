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
        public TermsAndConditionsService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<(Result Result, bool IsHealthAndSafetyMeasuresAccepted)> GetHealthAndSafetyMeasuresFor(string username)
        {
            var contacts = await Client.For<contact>()
                .Filter(c => c.gc_username == username)
                //.Filter(c => c.fullname == fullname)
                .Expand(c => c.gc_usersettings)
                .FindEntriesAsync();

            if (contacts.Count() == 0)
            {
                return (Result.Failure(new[] { $"No contacts with username '{username}'." }), default(bool));
            }

            if (contacts.Count() > 1)
            {
                return (Result.Failure(new[] { $"More than one contacts with username '{username}'." }), default(bool));
            }

            return (Result.Success(), contacts.First()?.gc_usersettings?.gc_healthsafety.HasValue ?? false);
        }

        public async Task<(Result Result, bool IsPrivacyActStatementAccepted)> GetPrivacyActStatementFor(string username)
        {
            var contacts = await Client.For<contact>()
                .Filter(c => c.gc_username == username)
                //.Filter(c => c.fullname == fullname)
                .Expand(c => c.gc_usersettings)
                .FindEntriesAsync();

            if (contacts.Count() == 0)
            {
                return (Result.Failure(new[] { $"No contacts with username '{username}'." }), default(bool));
            }

            if (contacts.Count() > 1)
            {
                return (Result.Failure(new[] { $"More than one contacts with username '{username}'." }), default(bool));
            }

            return (Result.Success(), contacts.First()?.gc_usersettings?.gc_privacystatement.HasValue ?? false);
        }

        public Task SetHealthAndSafetyMeasuresFor(string username, bool isHealthAndSafetyMeasuresAccepted)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Result> SetPrivacyActStatementFor(string username, bool isPrivateActStatementAccepted)
        {
            var contacts = await Client.For<contact>()
                .Filter(c => c.gc_username == username)
                //.Filter(c => c.fullname == fullname)
                .Expand(c => c.gc_usersettings)
                .FindEntriesAsync();

            if (contacts.Count() == 0)
            {
                return Result.Failure(new[] { $"No contacts with username '{username}'." });
            }

            if (contacts.Count() > 1)
            {
                return Result.Failure(new[] { $"More than one contacts with username '{username}'." });
            }

            var contact = contacts.First();

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
    }
}