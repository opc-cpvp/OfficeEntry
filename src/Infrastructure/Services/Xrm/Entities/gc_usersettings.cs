using OfficeEntry.Domain.Entities;
using System;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
    internal class gc_usersettings
    {
        public Guid gc_usersettingsid { get; set; }
        public DateTime? gc_healthsafety { get; set; }
        public DateTime? gc_privacystatement { get; set; }

        public static UserSettings Convert(gc_usersettings userSettings)
        {
            return new UserSettings
            {
                Id = userSettings.gc_usersettingsid,
                HealthSafety = userSettings.gc_healthsafety,
                PrivacyStatement = userSettings.gc_privacystatement
            };
        }
    }
}