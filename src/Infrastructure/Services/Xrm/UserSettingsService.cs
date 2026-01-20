using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class UserSettingsService : IUserSettingsService
{
    private readonly IODataClient _client;
    private readonly IUserService _userService;

    public UserSettingsService(IODataClient client, IUserService userService)
    {
        _client = client;
        _userService = userService;
    }

    public async Task<(Result Result, UserSettings UserSettings)> GetUserSettingsFor(Guid contactId)
    {
        var userSettings = await _client.For<contact>()
            .Key(contactId)
            .NavigateTo(c => c.gc_usersettings)
            .FindEntryAsync();

        var map = gc_usersettingses.Convert(userSettings);

        return (Result.Success(), map);
    }
}