using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System.Linq;

namespace OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests
{
    public class CreateAccessRequestCommand : IRequest
    {
        public AccessRequest AccessRequest { get; set; }
    }

    public class CreateAccessRequestCommandHandler : IRequestHandler<CreateAccessRequestCommand>
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly IBuildingRoleService _buildingRoleService;

        private IMediator _mediator;

        public CreateAccessRequestCommandHandler(
            IAccessRequestService accessRequestService,
            ICurrentUserService currentUserService,
            IUserService userService,
            IBuildingRoleService buildingRoleService,
            IMediator mediator
        ) {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _userService = userService;
            _buildingRoleService = buildingRoleService;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateAccessRequestCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var (_, contact) = await _userService.GetContactByUsername(username);

            if (contact.UserSettings?.HealthSafety is null || contact.UserSettings?.PrivacyStatement is null)
            {
                throw new Exception("Can't create an access request without accepting Privacy Act statement and Health and Safety measures");
            }

            var floorPlan = request.AccessRequest.FloorPlan;
            var floorId = floorPlan.Floor.Id;
            var date = request.AccessRequest.StartTime;
            var requestContactCount = (request.AccessRequest.Visitors?.Count ?? 0) + 1;

            var currentCapacities = await _mediator.Send(new GetSpotsAvailablePerHourQuery { FloorId = floorId, SelectedDay = date }, cancellationToken);

            var hasAvailableCapacity = currentCapacities
                .Where(x => x.Hour >= request.AccessRequest.StartTime.Hour && x.Hour < request.AccessRequest.EndTime.Hour)
                .All(x => x.Capacity - x.SpotsReserved - requestContactCount >= 0);

            if (!hasAvailableCapacity)
            {
                throw new Exception("Your request exceeds the floor capacity");
            }

            if (request.AccessRequest.Employee is null)
            {
                request.AccessRequest.Employee = contact;
            }
            else
            {
                request.AccessRequest.Delegate = contact;
            }

            var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(floorPlan.Id, DateOnly.FromDateTime(date));
            var approvedAccessRequests = accessRequests
                .Where(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Approved)
                .GroupBy(a => a.Employee.Id)
                .ToDictionary(g => g.Key, g => g.ToList());
            var pendingAccessRequests = accessRequests
                .Where(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Pending)
                .GroupBy(a => a.Employee.Id)
                .ToDictionary(g => g.Key, g => g.ToList());
            var (_, buildingRoles) = await _buildingRoleService.GetBuildingRolesFor(request.AccessRequest.Employee.Id);

            var isEmployeeFirstAidAttendant = buildingRoles.Any(b => b.Role.Key == (int)BuildingRole.BuildingRoles.FirstAidAttendant);
            var isEmployeeFloorEmergencyOfficer = buildingRoles.Any(b => b.Role.Key == (int)BuildingRole.BuildingRoles.FloorEmergencyOfficer);
            var employeeHasApprovedAccessRequest = approvedAccessRequests.ContainsKey(request.AccessRequest.Employee.Id);

            var buildingRoleTasks = approvedAccessRequests.Keys
                .Where(x => x != request.AccessRequest.Employee.Id)
                .Select(x => _buildingRoleService.GetBuildingRolesFor(x));

            var assignmentCounts = (await Task.WhenAll(buildingRoleTasks))
                .SelectMany(r => r.BuildingRoles)
                .Where(r => r.Floor?.Id == request.AccessRequest.Floor.Id)
                .GroupBy(r => (BuildingRole.BuildingRoles)r.Role.Key)
                .ToDictionary(g => g.Key, g => g.ToList().Count);

            assignmentCounts.TryGetValue(BuildingRole.BuildingRoles.FirstAidAttendant, out var count);
            var approvedFirstAidAttendants = count + (isEmployeeFirstAidAttendant ? 1 : 0);

            assignmentCounts.TryGetValue(BuildingRole.BuildingRoles.FloorEmergencyOfficer, out count);
            var approvedFloorEmergencyOfficers = count + (isEmployeeFloorEmergencyOfficer ? 1 : 0);

            var thresholdFirstAidAttendant = 50;
            var thresholdFloorEmergencyOfficer = 50;
            var minimumFirstAidAttendant = 5;
            var minimumFloorEmergencyOfficer = 10;

            var maxFirstAidAttendantCapacity = Math.Max(approvedFirstAidAttendants * thresholdFirstAidAttendant, minimumFirstAidAttendant);
            var maxFloorEmergencyOfficerCapacity = Math.Max(approvedFloorEmergencyOfficers * thresholdFloorEmergencyOfficer, minimumFloorEmergencyOfficer);
            var maxCapacity = Math.Min(maxFirstAidAttendantCapacity, maxFloorEmergencyOfficerCapacity);
            var currentCapacity = approvedAccessRequests.Keys.Count;
            var hasCapacity = currentCapacity < maxCapacity;

            if (isEmployeeFirstAidAttendant || isEmployeeFloorEmergencyOfficer)
            {
                request.AccessRequest.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;

                var remainingCapacity = maxCapacity - currentCapacity;
                var remainingAccessRequests = pendingAccessRequests
                    .Take(Math.Min(remainingCapacity, pendingAccessRequests.Count))
                    .SelectMany(x => x.Value);

                foreach (var remainingAccessRequest in remainingAccessRequests)
                {
                    remainingAccessRequest.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
                    await _accessRequestService.UpdateAccessRequest(remainingAccessRequest);
                }
            }
            else if (employeeHasApprovedAccessRequest || hasCapacity)
            {
                request.AccessRequest.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
            }
            else
            {
                var pendingCapacity = pendingAccessRequests.Keys.Count;
                var totalCapacity = currentCapacity + pendingCapacity + 1;

                var notifyFirstAidAttendants = totalCapacity == maxFirstAidAttendantCapacity;
                var notifyFloorEmergencyOfficers = totalCapacity == maxFloorEmergencyOfficerCapacity;
            }

            /*

            // FAA = 5 / 50 / 100
            // FEO = 10 / 50 / 100
            var isEmployeeFirstAidAttendant = buildingRoles.Any(b => b.Role.Key == (int)BuildingRole.BuildingRoles.FirstAidAttendant);
            if (isEmployeeFirstAidAttendant)
            {

                request.AccessRequest.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
                await _accessRequestService.CreateAccessRequest(request.AccessRequest);

                pendingAccessRequests.Take(Math.Min(thresholdFirstAidAttendant, pendingAccessRequests.Count()))

                return Unit.Value;
            }

            var isEmployeeFloorEmergencyOfficer = buildingRoles.Any(b => b.Role.Key == (int)BuildingRole.BuildingRoles.FloorEmergencyOfficer);

            var minimumFirstAidAttendant = 5;
            var minimumFloorEmergencyOfficer = 10;

            // FEO = 10 / 50 / 100
            */
            await _accessRequestService.CreateAccessRequest(request.AccessRequest);

            return Unit.Value;
        }
    }
}
