using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services
{
    public class TermsAndConditionsService : XrmService, ITermsAndConditionsService
    {
        public TermsAndConditionsService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<(Result Result, bool IsHealthAndSafetyMeasuresAccepted)> GetHealthAndSafetyMeasuresFor(string fullname)
        {
            var contacts = await Client.For<contact>()
                //.Filter(c => c.gc_username == fullname)
                .Filter(c => c.fullname == fullname)
                .Expand(c => c.gc_usersettings)
                .FindEntriesAsync();

            if (contacts.Count() == 0)
            {
                return (Result.Failure(new[] { $"No contacts with fullname '{fullname}'." }), default(bool));
            }

            if (contacts.Count() > 1)
            {
                return (Result.Failure(new[] { $"More than one contacts with fullname '{fullname}'." }), default(bool));
            }

            return (Result.Success(), contacts.First()?.gc_usersettings?.gc_healthsafety.HasValue ?? false);
        }

        public async Task<(Result Result, bool IsPrivacyActStatementAccepted)> GetPrivacyActStatementFor(string fullname)
        {
            var contacts = await Client.For<contact>()
                //.Filter(c => c.gc_username == fullname)
                .Filter(c => c.fullname == fullname)
                .Expand(c => c.gc_usersettings)
                .FindEntriesAsync();

            if (contacts.Count() == 0)
            {
                return (Result.Failure(new[] { $"No contacts with fullname '{fullname}'." }), default(bool));
            }

            if (contacts.Count() > 1)
            {
                return (Result.Failure(new[] { $"More than one contacts with fullname '{fullname}'." }), default(bool));
            }

            return (Result.Success(), contacts.First()?.gc_usersettings?.gc_privacystatement.HasValue ?? false);
        }

        public Task SetHealthAndSafetyMeasuresFor(string fullname, bool isHealthAndSafetyMeasuresAccepted)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Result> SetPrivacyActStatementFor(string fullname, bool isPrivateActStatementAccepted)
        {
            var contacts = await Client.For<contact>()
                //.Filter(c => c.gc_username == fullname)
                .Filter(c => c.fullname == fullname)
                .Expand(c => c.gc_usersettings)
                .FindEntriesAsync();

            if (contacts.Count() == 0)
            {
                return Result.Failure(new[] { $"No contacts with fullname '{fullname}'." });
            }

            if (contacts.Count() > 1)
            {
                return Result.Failure(new[] { $"More than one contacts with fullname '{fullname}'." });
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

        private protected class contact
        {
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string fullname { get; set; }
            public string emailaddress1 { get; set; }
            public string gc_username { get; set; }
            public string telephone1 { get; set; }
            public Guid contactid { get; set; }
            public gc_usersettings gc_usersettings { get; set; }
        }

        private protected class gc_usersettings
        {
            public Guid gc_usersettingsid { get; set; }
            public DateTime? gc_healthsafety { get; set; }
            public DateTime? gc_privacystatement { get; set; }
        }
    }
}