using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace OfficeEntry.Infrastructure.Services
{
    internal class NotificationService : INotificationService
    {
        private readonly IUserService _userService;
        private readonly IEmployeeRoleService _employeeRoleService;
        private readonly ILocationService _locationService;
        private readonly ITemplateService _templateService;
        private readonly IEmailService _emailService;

        public NotificationService(IUserService userService, IEmployeeRoleService employeeRoleService, ILocationService locationService, ITemplateService templateService, IEmailService emailService)
        {
            _userService = userService;
            _employeeRoleService = employeeRoleService;
            _locationService = locationService;
            _templateService = templateService;
            _emailService = emailService;
        }

        [SupportedOSPlatform("windows")]
        public async Task<Result> NotifyOfMaximumCapacityReached(CapacityNotification capacityNotification, EmployeeRoleType roleType)
        {
            var employeeRole = _employeeRoleService.GetEmployeeRole(roleType);
            capacityNotification.RoleFrenchName = employeeRole.FrenchName;
            capacityNotification.RoleEnglishName = employeeRole.EnglishName;

            var (_, sender) = await _userService.GetSystemUserByUsername(WindowsIdentity.GetCurrent().Name);
            var contacts = await _locationService.GetContactsForBuildingByRole(capacityNotification.Building.Id, roleType);

            if (!contacts.Any())
                    throw new Exception($"Failed to find any {employeeRole.EnglishName}");

            var description = _templateService.GetEmailTemplate(EmailTemplates.CapacityNotification, capacityNotification);
            var email = new Email
            {
                From = sender,
                To = contacts,
                Subject = capacityNotification.Title,
                Description = description
            };

            return await _emailService.SendEmailAsync(email);
        }

        [SupportedOSPlatform("windows")]
        public async Task<Result> NotifyOfAvailableCapacity(CapacityNotification capacityNotification, EmployeeRoleType roleType)
        {
            var employeeRole = _employeeRoleService.GetEmployeeRole(roleType);
            capacityNotification.RoleFrenchName = employeeRole.FrenchName;
            capacityNotification.RoleEnglishName = employeeRole.EnglishName;

            var (_, sender) = await _userService.GetSystemUserByUsername(WindowsIdentity.GetCurrent().Name);
            var contacts = await _locationService.GetContactsForBuildingByRole(capacityNotification.Building.Id, roleType);

            if (!contacts.Any())
                throw new Exception($"Failed to find any {capacityNotification.RoleEnglishName}");

            var description = _templateService.GetEmailTemplate(EmailTemplates.CapacityAvailableNotification, capacityNotification);
            var email = new Email
            {
                From = sender,
                To = contacts,
                Subject = capacityNotification.Title,
                Description = description
            };

            return await _emailService.SendEmailAsync(email);
        }

        [SupportedOSPlatform("windows")]
        public async Task<Result> NotifyAccessRequestEmployee(AccessRequestNotification accessRequestNotification)
        {
            var (_, sender) = await _userService.GetSystemUserByUsername(WindowsIdentity.GetCurrent().Name);

            var accessRequest = accessRequestNotification.AccessRequest;
            var recipients = new List<Contact> { accessRequest.Employee };

            if (accessRequest.Delegate is not null)
            {
                recipients.Add(accessRequest.Delegate);
            }

            var template = (AccessRequest.ApprovalStatus) accessRequestNotification.AccessRequest.Status.Key switch
            {
                AccessRequest.ApprovalStatus.Approved => EmailTemplates.ApprovedAccessRequestNotification,
                AccessRequest.ApprovalStatus.Pending => EmailTemplates.PendingAccessRequestNotification,
                AccessRequest.ApprovalStatus.Cancelled => EmailTemplates.CancelledAccessRequestNotification,
                _ => throw new Exception($"Unable to determine template using Access Request status: {accessRequestNotification.AccessRequest.Status.Key}")
            };

            var description = _templateService.GetEmailTemplate(template, accessRequestNotification);
            var email = new Email
            {
                From = sender,
                To = recipients,
                Subject = accessRequestNotification.Title,
                Description = description
            };

            return await _emailService.SendEmailAsync(email);
        }
    }
}
