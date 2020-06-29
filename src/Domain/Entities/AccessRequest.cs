using System;
using System.Collections.Generic;

namespace OfficeEntry.Domain.Entities
{
    public class AccessRequest
    {
        public Guid Id { get; set; }
        public Building Building { get; set; }
        public Contact Employee { get; set; }
        public string Details { get; set; }
        public DateTime EndTime { get; set; }
        public Floor Floor { get; set; }
        public Contact Manager { get; set; }
        public OptionSet Reason { get; set; }
        public DateTime StartTime { get; set; }
        public OptionSet Status { get; set; }
        public List<Contact> Visitors { get; set; }

        public enum ApprovalStatus
        {
            Pending = 948160000,
            Approved = 948160001,
            Declined = 948160002,
            Cancelled = 948160003
        }
    }
}