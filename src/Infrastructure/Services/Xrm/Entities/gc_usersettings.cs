using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

internal class gc_usersettingses
{
    public string gc_name { get; set; }
    public Guid gc_usersettingsid { get; set; }
    public DateTime? gc_healthsafety { get; set; }
    public DateTime? gc_privacystatement { get; set; }
    public int statecode { get; set; }
    public IList<contact> gc_usersettings_contact_usersettings { get; set; }

    public static UserSettings Convert(gc_usersettingses userSettings)
    {
        if (userSettings is null)
            return null;

        return new UserSettings
        {
            Id = userSettings.gc_usersettingsid,
            HealthSafety = userSettings.gc_healthsafety,
            PrivacyStatement = userSettings.gc_privacystatement
        };
    }
}
