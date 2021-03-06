﻿using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.User.Commands.UpdatePrivacyStatementRequests;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Controllers
{
    public class PrivacyStatementController : ApiController
    {
        [HttpPut]
        public async Task Put(bool isPrivateActStatementAccepted)
        {
            await Mediator.Send(new UpdatePrivacyActStatementForCurrentUserCommand { IsPrivacyActStatementAccepted = isPrivateActStatementAccepted });
        }
    }
}