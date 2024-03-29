﻿using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.Users.Queries.GetContactsRequests;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Controllers;

public class UsersController : ApiController
{
    [HttpGet]
    public async Task<IEnumerable<Contact>> Get()
    {
        return await Mediator.Send(GetContactsQuery.Instance);
    }
}
