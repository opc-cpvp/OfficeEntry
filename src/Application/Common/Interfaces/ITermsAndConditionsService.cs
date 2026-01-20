using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;

namespace OfficeEntry.Application.Common.Interfaces;

public interface ITermsAndConditionsService
{
    Task<TermsAndConditions> GetTermsAndConditionsFor(string username);

    Task<Result> SetPrivacyActStatementFor(string username, bool isPrivateActStatementAccepted);

    Task<Result> SetHealthAndSafetyMeasuresFor(string username, bool isHealthAndSafetyMeasuresAccepted);
}
