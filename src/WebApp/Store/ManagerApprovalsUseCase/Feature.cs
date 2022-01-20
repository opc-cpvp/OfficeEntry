using Fluxor;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Store.ManagerApprovalsUseCase
{
    public class Feature : Feature<ManagerApprovalsState>
    {
        public override string GetName() => "Manager Approvals";

        protected override ManagerApprovalsState GetInitialState() =>
            new ManagerApprovalsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
    }
}
