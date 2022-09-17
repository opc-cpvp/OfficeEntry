using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendEmail(Email email);
    }
}
