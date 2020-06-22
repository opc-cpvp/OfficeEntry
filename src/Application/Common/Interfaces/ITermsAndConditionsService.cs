using OfficeEntry.Application.Common.Models;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface ITermsAndConditionsService
    {
        Task<(Result Result, bool IsPrivacyActStatementAccepted)> GetPrivacyActStatementFor(string username);
        Task<Result> SetPrivacyActStatementFor(string username, bool isPrivateActStatementAccepted);

        Task<(Result Result, bool IsHealthAndSafetyMeasuresAccepted)> GetHealthAndSafetyMeasuresFor(string username);
        Task<Result> SetHealthAndSafetyMeasuresFor(string username, bool isHealthAndSafetyMeasuresAccepted);
    }
}
