using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task<Result> NotifyOfMaximumCapacityReached(CapacityNotification capacityNotification, EmployeeRoleType roleType);
        Task<Result> NotifyOfAvailableCapacity(CapacityNotification capacityNotification, EmployeeRoleType roleType);
        Task<Result> NotifyAccessRequestEmployee(AccessRequestNotification accessRequestNotification);
    }
}
