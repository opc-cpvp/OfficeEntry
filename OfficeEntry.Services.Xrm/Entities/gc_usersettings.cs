using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeEntry.Services.Xrm.Entities
{
    class gc_usersettings
    {
        public Guid gc_usersettingsid { get; set; }
        public DateTime? gc_healthsafety { get; set; }
        public DateTime? gc_privacystatement { get; set; }
    }
}
