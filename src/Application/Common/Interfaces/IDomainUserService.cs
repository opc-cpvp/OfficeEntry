using OfficeEntry.Domain.ValueObjects;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IDomainUserService
    {
        Task<string> GetUserNameAsync(AdAccount adAccount);
    }
}