using System.Runtime.Versioning;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using System.Security.Principal;

namespace OfficeEntry.Infrastructure.Services
{
    internal class NotificationService : INotificationService
    {
        private const string FirstAidAttendantEnglishName = "First Aid Attendant";
        private const string FirstAidAttendantFrenchName = "[FR] First Aid Attendant";
        private const string FloorEmergencyOfficerEnglishName = "Floor Emergency Officer";
        private const string FloorEmergencyOfficerFrenchName = "[FR] Floor Emergency Officer";

        private readonly IUserService _userService;
        private readonly IBuildingRoleService _buildingRoleService;
        private readonly ITemplateService _templateService;
        private readonly IEmailService _emailService;

        public NotificationService(IUserService userService, IBuildingRoleService buildingRoleService, ITemplateService templateService, IEmailService emailService)
        {
            _userService = userService;
            _buildingRoleService = buildingRoleService;
            _templateService = templateService;
            _emailService = emailService;
        }

        [SupportedOSPlatform("windows")]
        public async Task<Result> NotifyFirstAidAttendants(CapacityNotification capacityNotification)
        {
            capacityNotification.RoleEnglishName = FirstAidAttendantEnglishName;
            capacityNotification.RoleFrenchName = FirstAidAttendantFrenchName;

            var (_, sender) = await _userService.GetSystemUserByUsername(WindowsIdentity.GetCurrent().Name);
            var (_, firstAidAttendants) = await _buildingRoleService.GetContactsByBuildingRole(capacityNotification.Floor.Id, BuildingRole.BuildingRoles.FirstAidAttendant);

            if (!firstAidAttendants.Any())
                throw new Exception("Failed to find any First Aid Attendants");

            var description = _templateService.GetEmailTemplate(EmailTemplates.CapacityNotification, capacityNotification);
            var email = new Email
            {
                From = sender,
                To = firstAidAttendants,
                Subject = capacityNotification.Title,
                Description = description
            };

            return await _emailService.SendEmailAsync(email);
        }

        [SupportedOSPlatform("windows")]
        public async Task<Result> NotifyFloorEmergencyOfficers(CapacityNotification capacityNotification)
        {
            capacityNotification.RoleEnglishName = FloorEmergencyOfficerEnglishName;
            capacityNotification.RoleFrenchName = FloorEmergencyOfficerFrenchName;

            var (_, sender) = await _userService.GetSystemUserByUsername(WindowsIdentity.GetCurrent().Name);
            var (_, floorEmergencyOfficers) = await _buildingRoleService.GetContactsByBuildingRole(capacityNotification.Floor.Id, BuildingRole.BuildingRoles.FloorEmergencyOfficer);

            if (!floorEmergencyOfficers.Any())
                throw new Exception("Failed to find any Floor Emergency Officers");

            var description = _templateService.GetEmailTemplate(EmailTemplates.CapacityNotification, capacityNotification);
            var email = new Email
            {
                From = sender,
                To = floorEmergencyOfficers,
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

            var description = _templateService.GetEmailTemplate(EmailTemplates.AccessRequestNotification, accessRequestNotification);
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
