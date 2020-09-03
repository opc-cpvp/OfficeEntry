using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeEntry.WebApp.Store.TermsAndConditionsUseCase
{
    public class TermsAndConditionsState
    {
        public bool IsLoading { get; }

        public bool IsHealthAndSafetyMeasuresStatementAccepted { get; }
        public bool IsPrivacyActStatementAccepted { get; }

        public TermsAndConditionsState(bool isLoading, bool isHealthAndSafetyMeasuresStatementAccepted, bool isPrivacyActStatementAccepted)
        {
            IsLoading = isLoading;
            IsHealthAndSafetyMeasuresStatementAccepted = isHealthAndSafetyMeasuresStatementAccepted;
            IsPrivacyActStatementAccepted = isPrivacyActStatementAccepted;
        }
    }
}
