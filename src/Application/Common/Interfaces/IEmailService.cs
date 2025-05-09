using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(Email email);
    }
}
