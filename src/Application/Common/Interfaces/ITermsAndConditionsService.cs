using OfficeEntry.Application.Common.Models;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface ITermsAndConditionsService
    {
        Task<(Result Result, bool IsPrivacyActStatementAccepted)> GetPrivacyActStatementFor(string fullname);
        Task<Result> SetPrivacyActStatementFor(string fullname, bool isPrivateActStatementAccepted);

        Task<(Result Result, bool IsHealthAndSafetyMeasuresAccepted)> GetHealthAndSafetyMeasuresFor(string fullname);
        Task SetHealthAndSafetyMeasuresFor(string fullname, bool isHealthAndSafetyMeasuresAccepted);
    }
}
