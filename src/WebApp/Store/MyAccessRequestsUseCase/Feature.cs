using Fluxor;
using OfficeEntry.Domain.Entities;
using System;

namespace OfficeEntry.WebApp.Store.MyAccessRequestsUseCase
{
    public class Feature : Feature<MyAccessRequestsState>
    {
        public override string GetName() => "My Access Requests";

        protected override MyAccessRequestsState GetInitialState() =>
            new MyAccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
    }
}
