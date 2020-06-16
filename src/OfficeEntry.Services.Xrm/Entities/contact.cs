using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeEntry.Services.Xrm.Entities
{
    class contact
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string emailaddress1 { get; set; }
        public string gc_username { get; set; }
        public string telephone1 { get; set; }
        public Guid contactid { get; set; }
        public gc_usersettings gc_usersettings { get; set; }
    }
}
