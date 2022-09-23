using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task<Result> NotifyFirstAidAttendants(CapacityNotification capacityNotification);
        Task<Result> NotifyFloorEmergencyOfficers(CapacityNotification capacityNotification);
        Task<Result> NotifyAccessRequestEmployee(AccessRequestNotification accessRequestNotification);
    }
}
