using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.ValueObjects;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class IdentityController : ApiController
    {
        private readonly IDomainUserService _domainUserService;

        public IdentityController(IDomainUserService domainUserService)
        {
            _domainUserService = domainUserService;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return await _domainUserService.GetUserNameAsync(AdAccount.For(User.Identity.Name));
        }
    }
}
