using Moq;
using OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using Xunit;

namespace Application.UnitTests
{
    public class UpdateAccessRequestCommandTests
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly IBuildingRoleService _buildingRoleService;
        private readonly ILocationService _locationService;

        private AccessRequest _accessRequest;

        public UpdateAccessRequestCommandTests()
        {
            _accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = new Contact { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan
                {
                    Id = Guid.NewGuid(),
                    Floor = new Floor
                    {
                        Id = Guid.NewGuid()
                    }
                },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Cancelled }
            };

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.UpdateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync(Result.Success);

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 50,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 50,
                TotalCapacity = 50
            };

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            var buildingRoles = new List<BuildingRole>
            {
                new()
                {
                    Floor = new Floor { Id = _accessRequest.FloorPlan.Floor.Id },
                    Role = new OptionSet { Key = (int)BuildingRole.BuildingRoles.FirstAidAttendant }
                },
                new()
                {
                    Floor = new Floor { Id = _accessRequest.FloorPlan.Floor.Id },
                    Role = new OptionSet { Key = (int)BuildingRole.BuildingRoles.FloorEmergencyOfficer }
                }
            };

            var buildingRoleServiceMock = new Mock<IBuildingRoleService>();
            buildingRoleServiceMock.Setup(x => x.GetBuildingRolesFor(It.IsAny<Guid>())).ReturnsAsync((Result.Success(), buildingRoles));

            _accessRequestService = accessRequestServiceMock.Object;
            _buildingRoleService = buildingRoleServiceMock.Object;
            _locationService = locationServiceMock.Object;
        }

        [Fact]
        public async Task Should_notify_employee_when_the_access_request_status_has_been_approved()
        {
            // Arrange
            _accessRequest.Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Approved };

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);

            var request = new UpdateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new UpdateAccessRequestCommandHandler(_accessRequestService, _buildingRoleService, _locationService, notificationServiceMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>()), Times.Never);
            notificationServiceMock.Verify(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>()), Times.Never);
        }

        [Fact]
        public async Task Should_notify_first_aid_attendants_and_floor_emergency_officers_when_a_first_aid_attendant_cancels_and_capacity_has_been_exceeded()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);

            var request = new UpdateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new UpdateAccessRequestCommandHandler(_accessRequestService, _buildingRoleService, _locationService, notificationServiceMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>()), Times.Once);
        }

        [Fact]
        public async Task Shouldnt_notify_first_aid_attendants_or_floor_emergency_officers_when_the_access_request_has_been_cancelled_for_a_regular_user()
        {
            // Arrange
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);

            var buildingRoleServiceMock = new Mock<IBuildingRoleService>();
            buildingRoleServiceMock.Setup(x => x.GetBuildingRolesFor(It.IsAny<Guid>())).ReturnsAsync((Result.Success(), Enumerable.Empty<BuildingRole>()));

            var request = new UpdateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new UpdateAccessRequestCommandHandler(_accessRequestService, buildingRoleServiceMock.Object, _locationService, notificationServiceMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>()), Times.Never);
            notificationServiceMock.Verify(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>()), Times.Never);
        }

        [Fact]
        public async Task Shouldnt_notify_first_aid_attendants_or_floor_emergency_officers_when_the_access_request_has_been_cancelled_and_theres_remaining_capacity()
        {
            // Arrange
            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 0,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 0
            };

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);


            var request = new UpdateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new UpdateAccessRequestCommandHandler(_accessRequestService, _buildingRoleService, locationServiceMock.Object, notificationServiceMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>()), Times.Never);
            notificationServiceMock.Verify(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>()), Times.Never);
        }
    }
}
