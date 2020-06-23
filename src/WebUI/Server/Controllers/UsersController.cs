using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.Users.Queries.GetContactsRequests;
using OfficeEntry.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class UsersController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<Contact>> Get()
        {
            return await Mediator.Send(new GetContactsQuery());
        }
    }
}
