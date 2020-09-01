using Fluxor;
using OfficeEntry.Domain.Entities;
using System;

namespace OfficeEntry.WebApp.Store.ApprovalsUseCase
{
    public class Feature : Feature<ApprovalsState>
    {
        public override string GetName() => "Approvals";

        protected override ApprovalsState GetInitialState() =>
            new ApprovalsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
    }
}
