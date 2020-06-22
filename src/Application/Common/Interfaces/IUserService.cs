using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<(Result Result, Contact Contact)> GetContact(string username);
    }
}
