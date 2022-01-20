using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface ITermsAndConditionsService
    {
        Task<TermsAndConditions> GetTermsAndConditionsFor(string username);

        Task<Result> SetPrivacyActStatementFor(string username, bool isPrivateActStatementAccepted);

        Task<Result> SetHealthAndSafetyMeasuresFor(string username, bool isHealthAndSafetyMeasuresAccepted);
    }
}