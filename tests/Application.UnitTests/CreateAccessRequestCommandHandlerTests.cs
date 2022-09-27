using MediatR;
using Moq;
using OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using System.Collections.Immutable;
using Xunit;

namespace Application.UnitTests
{
    public class CreateAccessRequestCommandHandlerTests
    {
        private static readonly UserSettings UserSettings = new()
        {
            HealthSafety = DateTime.Now,
            PrivacyStatement = DateTime.Now
        };

        private static readonly Contact Contact = new()
        {
            Id = Guid.NewGuid(),
            Username = "USER",
            UserSettings = UserSettings
        };

        private static readonly Contact FirstAidAttendantContact = new()
        {
            Id = Guid.NewGuid(),
            Username = "FAA",
            UserSettings = UserSettings
        };

        private static readonly Contact FloorEmergencyOfficerContact = new()
        {
            Id = Guid.NewGuid(),
            Username = "FEO",
            UserSettings = UserSettings
        };

        private static readonly Guid FloorId = Guid.NewGuid();
        public static readonly BuildingRole FirstAidAttendantRole = new()
        {
            Role = new OptionSet { Key = (int)BuildingRole.BuildingRoles.FirstAidAttendant },
            Floor = new Floor { Id = FloorId }
        };

        public static readonly BuildingRole FloorEmergencyOfficerRole = new()
        {
            Role = new OptionSet { Key = (int)BuildingRole.BuildingRoles.FloorEmergencyOfficer },
            Floor = new Floor { Id = FloorId }
        };

        private static readonly IEnumerable<CurrentCapacity> CurrentCapacities = new List<CurrentCapacity>
        {
            new() { Hour = DateTime.Now.Hour, Capacity = 100, SpotsReserved = 50 }
        };

        private readonly AccessRequest _accessRequest;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly IBuildingRoleService _buildingRoleService;
        private readonly IMediator _mediator;

        public CreateAccessRequestCommandHandlerTests()
        {
            _accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan
                {
                    Floor = new Floor
                    {
                        Id = FloorId
                    }
                },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            // Setup default mocks
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.UserId).Returns(Contact.Username);

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), Contact));
            userServiceMock.Setup(x => x.GetContactByUsername(FirstAidAttendantContact.Username)).ReturnsAsync((Result.Success(), FirstAidAttendantContact));
            userServiceMock.Setup(x => x.GetContactByUsername(FloorEmergencyOfficerContact.Username)).ReturnsAsync((Result.Success(), FloorEmergencyOfficerContact));

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<GetSpotsAvailablePerHourQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CurrentCapacities);
            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateAccessRequestCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var buildingRoleServiceMock = new Mock<IBuildingRoleService>();
            buildingRoleServiceMock.Setup(x => x.GetBuildingRolesFor(It.IsAny<Guid>())).ReturnsAsync((Result.Success(), Enumerable.Empty<BuildingRole>()));
            buildingRoleServiceMock.Setup(x => x.GetBuildingRolesFor(FirstAidAttendantContact.Id)).ReturnsAsync((Result.Success(), new List<BuildingRole> { FirstAidAttendantRole }));
            buildingRoleServiceMock.Setup(x => x.GetBuildingRolesFor(FloorEmergencyOfficerContact.Id)).ReturnsAsync((Result.Success(), new List<BuildingRole> { FloorEmergencyOfficerRole }));

            // Initialize services
            _currentUserService = currentUserServiceMock.Object;
            _userService = userServiceMock.Object;
            _buildingRoleService = buildingRoleServiceMock.Object;
            _mediator = mediatorMock.Object;
        }

        [Fact]
        public async Task Should_throw_an_exception_when_the_user_has_not_accepted_the_health_and_safety_measures()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Username = string.Empty,
                UserSettings = new UserSettings { HealthSafety = null, PrivacyStatement = DateTime.Now }
            };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>()))
                .ReturnsAsync((Result.Success(), contact));

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(null, _currentUserService, userServiceMock.Object, null,
                null, null, null);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, CancellationToken.None));

            // Assert
            Assert.Equal(
                "Can't create an access request without accepting Privacy Act statement and Health and Safety measures",
                result.Message);
        }

        [Fact]
        public async Task Should_throw_an_exception_when_the_user_has_not_accepted_the_privacy_statement()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Username = string.Empty,
                UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = null }
            };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>()))
                .ReturnsAsync((Result.Success(), contact));

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(null, _currentUserService, userServiceMock.Object, null,
                null, null, null);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, CancellationToken.None));

            // Assert
            Assert.Equal(
                "Can't create an access request without accepting Privacy Act statement and Health and Safety measures",
                result.Message);
        }

        [Fact]
        public async Task Should_throw_an_exception_when_the_user_has_not_accepted_either_statements()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Username = string.Empty,
                UserSettings = new UserSettings { HealthSafety = null, PrivacyStatement = null }
            };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>()))
                .ReturnsAsync((Result.Success(), contact));

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(null, _currentUserService, userServiceMock.Object, null,
                null, null, null);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, CancellationToken.None));

            // Assert
            Assert.Equal(
                "Can't create an access request without accepting Privacy Act statement and Health and Safety measures",
                result.Message);
        }

        [Fact]
        public async Task Should_throw_an_exception_when_the_capacity_has_been_reached()
        {
            // Arrange
            var currentCapacities = new List<CurrentCapacity>
            {
                new() { Hour = _accessRequest.StartTime.Hour, Capacity = 100, SpotsReserved = 100 }
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<GetSpotsAvailablePerHourQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(currentCapacities);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(null, _currentUserService, _userService, null,
                null, null, mediatorMock.Object);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, CancellationToken.None));

            // Assert
            Assert.Equal(
                "Your request exceeds the floor capacity",
                result.Message);
        }

        [Fact]
        public async Task Should_automatically_approve_access_request_when_capacity_is_available()
        {
            // Arrange
            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 0,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 0
            };

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), _accessRequest));
            accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray<AccessRequest>.Empty);

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetWorkspaceAsync(It.IsAny<Guid>())).ReturnsAsync(_accessRequest.Workspace);
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(floorPlanCapacity);

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(accessRequestServiceMock.Object, _currentUserService, _userService, _buildingRoleService,
                locationServiceMock.Object, notificationServiceMock.Object, _mediator);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)_accessRequest.Status.Key);
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_automatically_approve_access_request_when_capacity_isnt_available_and_a_previously_approved_access_request_exists()
        {
            // Arrange
            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 5
            };

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), _accessRequest));
            accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray.Create(new AccessRequest { Employee = Contact, Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Approved }}));

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetWorkspaceAsync(It.IsAny<Guid>())).ReturnsAsync(_accessRequest.Workspace);
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(floorPlanCapacity);

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(accessRequestServiceMock.Object, _currentUserService, _userService, _buildingRoleService,
                locationServiceMock.Object, notificationServiceMock.Object, _mediator);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)_accessRequest.Status.Key);
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_automatically_approve_access_request_when_capacity_isnt_available_and_employee_is_a_first_aid_attendant()
        {
            // Arrange
            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 5
            };

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.UserId).Returns(FirstAidAttendantContact.Username);

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), _accessRequest));
            accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray<AccessRequest>.Empty);

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetWorkspaceAsync(It.IsAny<Guid>())).ReturnsAsync(_accessRequest.Workspace);
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(floorPlanCapacity);

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(accessRequestServiceMock.Object, currentUserServiceMock.Object, _userService, _buildingRoleService,
                locationServiceMock.Object, notificationServiceMock.Object, _mediator);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)_accessRequest.Status.Key);
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_automatically_approve_access_request_when_capacity_isnt_available_and_employee_is_a_floor_emergency_officer()
        {
            // Arrange
            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 5
            };

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.UserId).Returns(FloorEmergencyOfficerContact.Username);

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), _accessRequest));
            accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray<AccessRequest>.Empty);

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetWorkspaceAsync(It.IsAny<Guid>())).ReturnsAsync(_accessRequest.Workspace);
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(floorPlanCapacity);

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(accessRequestServiceMock.Object, currentUserServiceMock.Object, _userService, _buildingRoleService,
                locationServiceMock.Object, notificationServiceMock.Object, _mediator);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)_accessRequest.Status.Key);
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_automatically_approve_pending_access_requests_when_a_first_aid_attendant_registers()
        {
            // Arrange
            var initialFloorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 7
            };

            var updatedFloorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 6,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 8
            };

            var finalFloorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 8,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 8
            };

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.UserId).Returns(FirstAidAttendantContact.Username);

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), _accessRequest));
            accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray.Create(
                    new AccessRequest { Employee = Contact, Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending } },
                    new AccessRequest { Employee = Contact, Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending } }
                ));

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetWorkspaceAsync(It.IsAny<Guid>())).ReturnsAsync(_accessRequest.Workspace);
            locationServiceMock.SetupSequence(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(initialFloorPlanCapacity)
                .ReturnsAsync(updatedFloorPlanCapacity)
                .ReturnsAsync(finalFloorPlanCapacity);

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<GetSpotsAvailablePerHourQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CurrentCapacities);
            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateAccessRequestCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(accessRequestServiceMock.Object, currentUserServiceMock.Object, _userService, _buildingRoleService,
                locationServiceMock.Object, notificationServiceMock.Object, mediatorMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)_accessRequest.Status.Key);
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            mediatorMock.Verify(x => x.Send(It.IsAny<UpdateAccessRequestCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Should_notify_first_aid_attendants_when_capacity_has_been_reached()
        {
            // Arrange
            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 5
            };

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), _accessRequest));
            accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray<AccessRequest>.Empty);

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetWorkspaceAsync(It.IsAny<Guid>())).ReturnsAsync(_accessRequest.Workspace);
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(floorPlanCapacity);

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(accessRequestServiceMock.Object, _currentUserService, _userService, _buildingRoleService,
                locationServiceMock.Object, notificationServiceMock.Object, _mediator);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Pending, (AccessRequest.ApprovalStatus)_accessRequest.Status.Key);
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_notify_floor_emergency_officers_when_capacity_has_been_reached()
        {
            // Arrange
            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 10,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 10
            };

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), _accessRequest));
            accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray<AccessRequest>.Empty);

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetWorkspaceAsync(It.IsAny<Guid>())).ReturnsAsync(_accessRequest.Workspace);
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(floorPlanCapacity);

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(accessRequestServiceMock.Object, _currentUserService, _userService, _buildingRoleService,
                locationServiceMock.Object, notificationServiceMock.Object, _mediator);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Pending, (AccessRequest.ApprovalStatus)_accessRequest.Status.Key);
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>()), Times.Once);
        }



        [Fact]
        public async Task Should_notify_first_aid_attendants_and_floor_emergency_officers_when_capacity_has_been_reached()
        {
            // Arrange
            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 50,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 50,
                TotalCapacity = 50
            };

            var accessRequestServiceMock = new Mock<IAccessRequestService>();
            accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), _accessRequest));
            accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray<AccessRequest>.Empty);

            var locationServiceMock = new Mock<ILocationService>();
            locationServiceMock.Setup(x => x.GetWorkspaceAsync(It.IsAny<Guid>())).ReturnsAsync(_accessRequest.Workspace);
            locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(floorPlanCapacity);

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);
            notificationServiceMock.Setup(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>())).ReturnsAsync(Result.Success);

            var request = new CreateAccessRequestCommand { AccessRequest = _accessRequest };
            var handler = new CreateAccessRequestCommandHandler(accessRequestServiceMock.Object, _currentUserService, _userService, _buildingRoleService,
                locationServiceMock.Object, notificationServiceMock.Object, _mediator);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Pending, (AccessRequest.ApprovalStatus)_accessRequest.Status.Key);
            notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>()), Times.Once);
            notificationServiceMock.Verify(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>()), Times.Once);
        }
    }
}
