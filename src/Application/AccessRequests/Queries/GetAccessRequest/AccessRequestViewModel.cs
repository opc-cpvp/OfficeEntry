using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest
{
    public class AccessRequestViewModel
    {
        public bool IsEmployee { get; set; }
        public bool IsManager { get; set; }
        public AccessRequest AccessRequest { get; set; }
    }
}
