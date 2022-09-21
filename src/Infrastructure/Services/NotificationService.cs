using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using System.Security.Principal;

namespace OfficeEntry.Infrastructure.Services
{
    internal class NotificationService : INotificationService
    {
        private const string NotificationSubject = "Entrée au bureau - [FR] Capacity has been reached or exceeded / Office Entry - Capacity has been reached or exceeded";
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

        public async Task<Result> NotifyFirstAidAttendants(Notification notification)
        {
            notification.Title = NotificationSubject;
            notification.RoleEnglishName = FirstAidAttendantEnglishName;
            notification.RoleFrenchName = FirstAidAttendantFrenchName;

            var (_, sender) = await _userService.GetSystemUserByUsername(WindowsIdentity.GetCurrent().Name);
            var (_, firstAidAttendants) = await _buildingRoleService.GetContactsByBuildingRole(notification.Floor.Id, BuildingRole.BuildingRoles.FirstAidAttendant);

            if (!firstAidAttendants.Any())
                throw new Exception("Failed to find any First Aid Attendants");

            var description = _templateService.GetEmailTemplate(EmailTemplates.Notification, notification);
            var email = new Email
            {
                From = sender,
                To = firstAidAttendants,
                Subject = NotificationSubject,
                Description = description
            };

            return await _emailService.SendEmailAsync(email);
        }

        public async Task<Result> NotifyFloorEmergencyOfficers(Notification notification)
        {
            notification.Title = NotificationSubject;
            notification.RoleEnglishName = FloorEmergencyOfficerEnglishName;
            notification.RoleFrenchName = FloorEmergencyOfficerFrenchName;

            var (_, sender) = await _userService.GetSystemUserByUsername(WindowsIdentity.GetCurrent().Name);
            var (_, floorEmergencyOfficers) = await _buildingRoleService.GetContactsByBuildingRole(notification.Floor.Id, BuildingRole.BuildingRoles.FloorEmergencyOfficer);

            if (!floorEmergencyOfficers.Any())
                throw new Exception("Failed to find any Floor Emergency Officers");

            var description = _templateService.GetEmailTemplate(EmailTemplates.Notification, notification);
            var email = new Email
            {
                From = sender,
                To = floorEmergencyOfficers,
                Subject = NotificationSubject,
                Description = description
            };

            return await _emailService.SendEmailAsync(email);
        }
    }
}
