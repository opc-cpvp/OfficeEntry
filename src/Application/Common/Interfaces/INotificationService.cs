using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task<Result> NotifyFirstAidAttendants(Notification notification);
        Task<Result> NotifyFloorEmergencyOfficers(Notification notification);
    }
}
