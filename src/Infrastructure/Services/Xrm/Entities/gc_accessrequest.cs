using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
    internal class gc_accessrequest
    {
        public string gc_name { get; set; }
        public Guid gc_accessrequestid { get; set; }
        public ApprovalStatus gc_approvalstatus { get; set; }
        public gc_building gc_building { get; set; }
        public string gc_details { get; set; }
        public contact gc_employee { get; set; }
        public DateTime gc_endtime { get; set; }
        public gc_floor gc_floor { get; set; }
        public contact gc_manager { get; set; }
        public AccessReasons gc_accessreason { get; set; }
        public DateTime gc_starttime { get; set; }
        public IList<contact> gc_accessrequest_contact_visitors { get; set; }
        public int statecode { get; set; }

        public static AccessRequest Convert(gc_accessrequest accessRequest) {
            return new AccessRequest
            {
                Id = accessRequest.gc_accessrequestid,
                Building = gc_building.Convert(accessRequest.gc_building),
                Employee = contact.Convert(accessRequest.gc_employee),
                Details = accessRequest.gc_details,
                EndTime = accessRequest.gc_endtime,
                Floor = gc_floor.Convert(accessRequest.gc_floor),
                Manager = contact.Convert(accessRequest.gc_manager),
                Reason = new OptionSet
                {
                    Key = (int)accessRequest.gc_accessreason,
                    Value = Enum.GetName(typeof(AccessReasons), accessRequest.gc_accessreason)
                },
                StartTime = accessRequest.gc_starttime,
                Status = new OptionSet
                {
                    Key = (int)accessRequest.gc_approvalstatus,
                    Value = Enum.GetName(typeof(ApprovalStatus), accessRequest.gc_approvalstatus)
                },
                Visitors = accessRequest.gc_accessrequest_contact_visitors.Select(v => contact.Convert(v)).ToList()
            };
        }
    }
}