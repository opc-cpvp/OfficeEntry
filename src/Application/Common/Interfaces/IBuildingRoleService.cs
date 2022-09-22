﻿using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IBuildingRoleService
    {
        Task<(Result Result, IEnumerable<BuildingRole> BuildingRoles)> GetBuildingRolesFor(Guid contactId);
        Task<(Result Result, IEnumerable<Contact> Contacts)> GetContactsByBuildingRole(Guid floorId, BuildingRole.BuildingRoles role);
    }
}