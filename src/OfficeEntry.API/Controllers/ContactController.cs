using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeEntry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/<LocationController>
        [HttpGet("/api/usersettings")]
        public Task<UserSettings> GetUserSettings()
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name.ToUpper();
            if (username.Contains("\\"))
                username = username.Substring(username.LastIndexOf("\\") + 1);

            return _userService.GetUserSettingsAsync(username);
        }

        // GET api/<LocationController>/5
        [HttpGet("/api/contacts")]
        public Task<IEnumerable<Contact>> GetContacts()
        {
            return _userService.GetContactsAsync();
        }
    }
}