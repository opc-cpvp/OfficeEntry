using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using OfficeEntry.Domain.ValueObjects;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task<Result> NotifyOfMaximumCapacityReached(CapacityNotification capacityNotification, EmployeeRoleType roleType);
        Task<Result> NotifyOfAvailableCapacity(CapacityNotification capacityNotification, EmployeeRoleType roleType);
        Task<Result> NotifyAccessRequestEmployee(AccessRequestNotification accessRequestNotification);
    }
}
