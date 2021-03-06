﻿using Microsoft.OData.Edm;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services.Xrm
{
    public class TermsAndConditionsService : XrmService, ITermsAndConditionsService
    {
        private readonly IUserService _userService;

        public TermsAndConditionsService(IHttpClientFactory httpClientFactory, IUserService contactService)
            : base(httpClientFactory)
        {
            _userService = contactService;
        }

        public async Task<TermsAndConditions> GetTermsAndConditionsFor(string username)
        {
            var (_, contact) = await _userService.GetContact(username);

            return new TermsAndConditions()
            {
                IsHealthAndSafetyMeasuresAccepted = contact.UserSettings?.HealthSafety.HasValue ?? false,
                IsPrivacyActStatementAccepted = contact.UserSettings?.PrivacyStatement.HasValue ?? false,
            };
        }

        public async Task<Result> SetHealthAndSafetyMeasuresFor(string username, bool isHealthAndSafetyMeasuresAccepted)
        {
            var (result, contact) = await _userService.GetContact(username);

            if (!result.Succeeded)
                return (result);

            await (contact.UserSettings is null ? Create() : Update(contact.UserSettings.Id));

            return Result.Success();

            async Task<gc_usersettingses> Create()
            {
                var userSettings = new gc_usersettingses
                {
                    gc_usersettingsid = Guid.NewGuid(),
                    gc_name = contact.Username,
                    gc_healthsafety = DateTime.Now
                };

                userSettings = await Client
                    .For<gc_usersettingses>()
                    .Set(userSettings)
                    .InsertEntryAsync();

                await Client
                    .For<contact>()
                    .Key(contact.Id)
                    .Set(new { gc_usersettings = userSettings })
                    .UpdateEntryAsync();

                return userSettings;
            }

            async Task<gc_usersettingses> Update(Guid id)
            {
                var userSettings = await Client
                    .For<gc_usersettingses>()
                    .Key(id)
                    .Set(new { gc_healthsafety = DateTime.Now })
                    .UpdateEntryAsync();

                return userSettings;
            }
        }

        public async Task<Result> SetPrivacyActStatementFor(string username, bool isPrivateActStatementAccepted)
        {
            var (result, contact) = await _userService.GetContact(username);

            if (!result.Succeeded)
                return (result);

            await (contact.UserSettings is null ? Create() : Update(contact.UserSettings.Id));

            return Result.Success();

            async Task<gc_usersettingses> Create()
            {
                var userSettings = new gc_usersettingses
                {
                    gc_usersettingsid = Guid.NewGuid(),
                    gc_name = contact.Username,
                    gc_privacystatement = DateTime.Now
                };

                userSettings = await Client
                    .For<gc_usersettingses>()
                    .Set(userSettings)
                    .InsertEntryAsync();

                await Client
                    .For<contact>()
                    .Key(contact.Id)
                    .Set(new { gc_usersettings = userSettings })
                    .UpdateEntryAsync();

                return userSettings;
            }

            async Task<gc_usersettingses> Update(Guid id)
            {
                var userSettings = await Client
                    .For<gc_usersettingses>()
                    .Key(id)
                    .Set(new { gc_privacystatement = DateTime.Now })
                    .UpdateEntryAsync();

                return userSettings;
            }
        }

        protected override void Dispose(bool disposing)
        {
            (_userService as IDisposable)?.Dispose();

            base.Dispose(disposing);
        }
    }
}