﻿using System.Collections.Immutable;
using MediatR;
using Moq;
using OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using Xunit;

namespace Application.UnitTests
{
    public class CreateAccessRequestCommandHandlerTests
    {
        private static readonly Guid FirstAidAttendantId = Guid.NewGuid();
        private static readonly Contact FirstAidAttendantContact = new()
        {
            Id = FirstAidAttendantId,
            UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = DateTime.Now }
        };

        private static readonly Guid FloorEmergencyOfficerId = Guid.NewGuid();
        private static readonly Contact FloorEmergencyOfficerContact = new()
        {
            Id = FloorEmergencyOfficerId,
            UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = DateTime.Now }
        };

        private readonly CreateAccessRequestCommandHandler _sut;
        private readonly Mock<IAccessRequestService> _accessRequestServiceMock = new Mock<IAccessRequestService>();
        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new Mock<ICurrentUserService>();
        private readonly Mock<ILocationService> _locationServiceMock = new Mock<ILocationService>();
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();

        public CreateAccessRequestCommandHandlerTests()
        {
            _locationServiceMock.Setup(x => x.GetFirstAidAttendantsAsync(It.IsAny<Guid>())).ReturnsAsync(new[] { FirstAidAttendantContact });
            _locationServiceMock.Setup(x => x.GetFloorEmergencyOfficersAsync(It.IsAny<Guid>())).ReturnsAsync(new[] { FloorEmergencyOfficerContact });

            _notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);

            _mediatorMock.Setup(x => x.Send(It.IsAny<UpdateAccessRequestCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            _sut = new CreateAccessRequestCommandHandler(_accessRequestServiceMock.Object,
                _currentUserServiceMock.Object, _userServiceMock.Object, _locationServiceMock.Object,
                _notificationServiceMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Should_throw_an_exception_when_the_user_has_not_accepted_the_health_and_safety_measures()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                UserSettings = new UserSettings { HealthSafety = null, PrivacyStatement = DateTime.Now }
            };

            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), contact));

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None));

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
                UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = null }
            };

            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), contact));

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None));

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
                UserSettings = new UserSettings { HealthSafety = null, PrivacyStatement = null }
            };

            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), contact));

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None));

            // Assert
            Assert.Equal(
                "Can't create an access request without accepting Privacy Act statement and Health and Safety measures",
                result.Message);
        }

        [Fact]
        public async Task Should_automatically_approve_access_request_when_capacity_is_available()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = DateTime.Now }
            };

            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 0,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 0
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), contact));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(ImmutableArray<AccessRequest>.Empty);
            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)accessRequest.Status.Key);
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_automatically_approve_access_request_when_capacity_isnt_available_and_a_previously_approved_access_request_exists()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = DateTime.Now }
            };

            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            var approvedAccessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Approved }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 5
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), contact));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(ImmutableArray.Create(approvedAccessRequest));
            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)accessRequest.Status.Key);
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_automatically_approve_access_request_when_capacity_isnt_available_and_employee_is_a_first_aid_attendant()
        {
            // Arrange
            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = FirstAidAttendantContact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 5
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), FirstAidAttendantContact));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(ImmutableArray<AccessRequest>.Empty);
            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)accessRequest.Status.Key);
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_automatically_approve_access_request_when_capacity_isnt_available_and_employee_is_a_floor_emergency_officer()
        {
            // Arrange
            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = FloorEmergencyOfficerContact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 10,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 10
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), FloorEmergencyOfficerContact));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(ImmutableArray<AccessRequest>.Empty);
            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)accessRequest.Status.Key);
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_automatically_approve_pending_access_requests_when_a_first_aid_attendant_registers()
        {
            // Arrange
            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = FirstAidAttendantContact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

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

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), FirstAidAttendantContact));
            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray.Create(
                    new AccessRequest { Employee = new Contact { Id = Guid.NewGuid() }, Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending } },
                    new AccessRequest { Employee = new Contact { Id = Guid.NewGuid() }, Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending } }
                ));
            _locationServiceMock.SetupSequence(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(initialFloorPlanCapacity)
                .ReturnsAsync(updatedFloorPlanCapacity)
                .ReturnsAsync(finalFloorPlanCapacity);

            // Act
            await _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Approved, (AccessRequest.ApprovalStatus)accessRequest.Status.Key);
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateAccessRequestCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Should_notify_first_aid_attendants_when_capacity_has_been_reached()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = DateTime.Now }
            };

            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 5
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), contact));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(ImmutableArray<AccessRequest>.Empty);
            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Pending, (AccessRequest.ApprovalStatus)accessRequest.Status.Key);
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            _notificationServiceMock.Verify(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_notify_floor_emergency_officers_when_capacity_has_been_reached()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = DateTime.Now }
            };

            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 10,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 10
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), contact));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(ImmutableArray<AccessRequest>.Empty);
            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Pending, (AccessRequest.ApprovalStatus)accessRequest.Status.Key);
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            _notificationServiceMock.Verify(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_notify_first_aid_attendants_and_floor_emergency_officers_when_capacity_has_been_reached()
        {
            // Arrange
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                UserSettings = new UserSettings { HealthSafety = DateTime.Now, PrivacyStatement = DateTime.Now }
            };

            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = contact,
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 50,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 50,
                TotalCapacity = 50
            };

            _userServiceMock.Setup(x => x.GetContactByUsername(It.IsAny<string>())).ReturnsAsync((Result.Success(), contact));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(ImmutableArray<AccessRequest>.Empty);
            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new CreateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            Assert.Equal(AccessRequest.ApprovalStatus.Pending, (AccessRequest.ApprovalStatus)accessRequest.Status.Key);
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            _notificationServiceMock.Verify(x => x.NotifyFirstAidAttendants(It.IsAny<CapacityNotification>()), Times.Once);
            _notificationServiceMock.Verify(x => x.NotifyFloorEmergencyOfficers(It.IsAny<CapacityNotification>()), Times.Once);
        }
    }
}
