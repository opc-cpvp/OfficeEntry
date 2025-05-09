using Moq;
using OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using OfficeEntry.Domain.Services;
using OfficeEntry.Domain.ValueObjects;
using System.Collections.Immutable;
using Xunit;

namespace Application.UnitTests
{
    public class UpdateAccessRequestCommandTests
    {
        private readonly UpdateAccessRequestCommandHandler _sut;
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<IAccessRequestService> _accessRequestServiceMock = new Mock<IAccessRequestService>();
        private readonly Mock<ILocationService> _locationServiceMock = new Mock<ILocationService>();

        private static readonly Guid EmergencyContactId = Guid.NewGuid();
        private static readonly Contact EmergencyContact = new()
        {
            Id = EmergencyContactId
        };

        public UpdateAccessRequestCommandTests()
        {
            _notificationServiceMock.Setup(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>())).ReturnsAsync(Result.Success);
            _notificationServiceMock.Setup(x => x.NotifyOfMaximumCapacityReached(It.IsAny<CapacityNotification>(), EmployeeRoleType.FirstAidAttendant)).ReturnsAsync(Result.Success);
            _notificationServiceMock.Setup(x => x.NotifyOfMaximumCapacityReached(It.IsAny<CapacityNotification>(), EmployeeRoleType.FloorEmergencyOfficer)).ReturnsAsync(Result.Success);

            _accessRequestServiceMock.Setup(x => x.UpdateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync(Result.Success);

            _locationServiceMock.Setup(x => x.GetFirstAidAttendantsAsync(It.IsAny<Guid>())).ReturnsAsync(new [] { EmergencyContact });
            _locationServiceMock.Setup(x => x.GetFloorEmergencyOfficersAsync(It.IsAny<Guid>())).ReturnsAsync(new[] { EmergencyContact });

            _sut = new UpdateAccessRequestCommandHandler(_accessRequestServiceMock.Object, _locationServiceMock.Object, _notificationServiceMock.Object);
        }

        [Fact]
        public async Task Should_notify_employee_when_the_access_request_status_has_been_cancelled()
        {
            // Arrange
            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = new Contact { Id = Guid.NewGuid() },
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan
                {
                    Id = Guid.NewGuid()
                },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Cancelled }
            };

            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(new FloorPlanCapacity());

            // Act
            await _sut.Handle(new UpdateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
        }

        [Fact]
        public async Task Should_notify_first_aid_attendants_when_a_first_aid_attendant_cancels_and_capacity_has_been_exceeded()
        {
            // Arrange
            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = new Contact { Id = EmergencyContactId },
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Cancelled }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 6,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 6
            };

            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new UpdateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            _notificationServiceMock.Verify(x => x.NotifyOfMaximumCapacityReached(It.IsAny<CapacityNotification>(), EmployeeRoleType.FirstAidAttendant), Times.Once);
            _notificationServiceMock.Verify(x => x.NotifyOfMaximumCapacityReached(It.IsAny<CapacityNotification>(), EmployeeRoleType.FloorEmergencyOfficer), Times.Never);
        }

        [Fact]
        public async Task Should_notify_floor_emergency_officers_when_a_floor_emergency_officers_cancels_and_capacity_has_been_exceeded()
        {
            // Arrange
            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = new Contact { Id = EmergencyContactId },
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Cancelled }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 11,
                MaxFirstAidAttendantCapacity = 50,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 11
            };

            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);

            // Act
            await _sut.Handle(new UpdateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            _notificationServiceMock.Verify(x => x.NotifyOfMaximumCapacityReached(It.IsAny<CapacityNotification>(), EmployeeRoleType.FirstAidAttendant), Times.Never);
            _notificationServiceMock.Verify(x => x.NotifyOfMaximumCapacityReached(It.IsAny<CapacityNotification>(), EmployeeRoleType.FloorEmergencyOfficer), Times.Once);
        }

        [Fact]
        public async Task Shouldnt_notify_emergency_contacts_when_an_access_request_has_been_cancelled_for_a_regular_user()
        {
            // Arrange
            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Employee = new Contact { Id = Guid.NewGuid() },
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Cancelled }
            };

            var floorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 0,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 0
            };

            _locationServiceMock.Setup(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(floorPlanCapacity);
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>())).ReturnsAsync(ImmutableArray<AccessRequest>.Empty);

            // Act
            await _sut.Handle(new UpdateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.IsAny<AccessRequestNotification>()), Times.Once);
            _notificationServiceMock.Verify(x => x.NotifyOfMaximumCapacityReached(It.IsAny<CapacityNotification>(), EmployeeRoleType.FirstAidAttendant), Times.Never);
            _notificationServiceMock.Verify(x => x.NotifyOfMaximumCapacityReached(It.IsAny<CapacityNotification>(), EmployeeRoleType.FloorEmergencyOfficer), Times.Never);
        }

        [Fact]
        public async Task Should_approve_pending_access_request_when_capacity_is_availabt()
        {
            // Arrange
            var accessRequest = new AccessRequest
            {
                Id = Guid.NewGuid(),
                Building = new Building { Id = Guid.NewGuid() },
                Floor = new Floor { Id = Guid.NewGuid() },
                CreatedOn = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                FloorPlan = new FloorPlan { Id = Guid.NewGuid() },
                Workspace = new Workspace { Id = Guid.NewGuid() },
                Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Cancelled }
            };

            var updatedFloorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 4,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 6
            };

            var finalFloorPlanCapacity = new FloorPlanCapacity
            {
                CurrentCapacity = 5,
                MaxFirstAidAttendantCapacity = 5,
                MaxFloorEmergencyOfficerCapacity = 10,
                TotalCapacity = 6
            };

            var accessRequestNotification = new AccessRequestNotification
            {
                AccessRequest = accessRequest
            };

            _accessRequestServiceMock.Setup(x => x.CreateAccessRequest(It.IsAny<AccessRequest>())).ReturnsAsync((Result.Success(), accessRequest));
            _accessRequestServiceMock.Setup(x => x.GetApprovedOrPendingAccessRequestsByFloorPlan(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(ImmutableArray.Create(
                    new AccessRequest { Employee = new Contact { Id = Guid.NewGuid() }, Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending } },
                    new AccessRequest { Employee = new Contact { Id = Guid.NewGuid() }, Status = new OptionSet { Key = (int)AccessRequest.ApprovalStatus.Pending } }
                ));
            _locationServiceMock.SetupSequence(x => x.GetCapacityByFloorPlanAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(updatedFloorPlanCapacity)
                .ReturnsAsync(finalFloorPlanCapacity);

            // Act
            await _sut.Handle(new UpdateAccessRequestCommand { AccessRequest = accessRequest }, CancellationToken.None);

            // Assert
            _notificationServiceMock.Verify(x => x.NotifyAccessRequestEmployee(It.Is<AccessRequestNotification>(x => x.AccessRequest != accessRequestNotification.AccessRequest)), Times.Once);
        }
    }
}
