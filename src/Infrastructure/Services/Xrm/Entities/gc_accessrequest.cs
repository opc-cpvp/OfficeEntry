using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
    internal class gc_accessrequest_contact_visitor
    {
        [Key]
        public Guid gc_accessrequest_contact_visitorid { get; set; }

        public Guid gc_accessrequestid { get; set; }

        public Guid contactid { get; set; }

        //public Guid gc_accessrequest_contact_visitorid { get; set; }
    }

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
        //public IList<contact> gc_accessrequest_contact_visitors { get; set; } = new List<contact>();
        public IList<gc_assetrequest> gc_accessrequest_assetrequest { get; set; } = new List<gc_assetrequest>();
        public int statecode { get; set; }

        public IList<gc_accessrequest_contact_visitor> gc_accessrequest_contact_visitors { get; set; } = new List<gc_accessrequest_contact_visitor>();

        public static AccessRequest Convert(gc_accessrequest accessRequest)
        {
            return new AccessRequest
            {
                Id = accessRequest.gc_accessrequestid,
                AssetRequests = accessRequest.gc_accessrequest_assetrequest.Select(a => gc_assetrequest.Convert(a)).ToList(),
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
                //Visitors = accessRequest.gc_accessrequest_contact_visitors.Select(v => contact.Convert(v)).ToList()
            };
        }

        public static gc_accessrequest MapFrom(AccessRequest accessRequest)
        {
            return new gc_accessrequest
            {
                gc_name = $"{accessRequest.Employee.FullName} - {accessRequest.StartTime:yyyy-MM-dd}",
                gc_accessreason = (AccessReasons)accessRequest.Reason.Key,
                gc_approvalstatus = ApprovalStatus.Pending,
                gc_building = new gc_building { gc_buildingid = accessRequest.Building.Id },
                gc_details = accessRequest.Details,
                gc_employee = new contact { contactid = accessRequest.Employee.Id },
                gc_endtime = accessRequest.EndTime,
                gc_floor = new gc_floor { gc_floorid = accessRequest.Floor.Id },
                gc_manager = new contact { contactid = accessRequest.Manager.Id },
                gc_starttime = accessRequest.StartTime
            };
        }
    }
}