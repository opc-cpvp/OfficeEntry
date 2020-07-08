using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using OfficeEntry.Domain.Entities;
using OfficeEntry.WebUI.Client.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Linq;
using OfficeEntry.Domain.Enums;

namespace OfficeEntry.WebUI.Client.Pages
{
    public abstract class SubmitAccessRequestBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public bool SurveyCompleted { get; set; }

        public bool IsLoaded { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            var isPrivacyActStatementAccepted = await Http.GetFromJsonAsync<bool>("api/PrivacyStatement");
            var isHealthAndSafetyStatementAccepted = await Http.GetFromJsonAsync<bool>("api/HealthAndSafetyMeasures");

            if (!isPrivacyActStatementAccepted)
            {
                NavigationManager.NavigateTo("/privacy-act-statement");
            }
            if (!isHealthAndSafetyStatementAccepted)
            {
                NavigationManager.NavigateTo("/health-and-safety-measures");
            }

            IsLoaded = true;

            StateHasChanged();
        }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            SurveyCompleted = true;

            var submission = JsonConvert.DeserializeObject<AccessRequestSubmission>(surveyResult);

            var accessRequest = new Domain.Entities.AccessRequest
            {
                AssetRequests = new List<AssetRequest>(),
                Building = new Building { Id = submission.building },
                Details = submission.details,
                EndTime = submission.startDate.AddHours(submission.endTime),
                Floor = new Floor { Id = submission.floor },
                Manager = new Contact { Id = submission.manager },
                Reason = new OptionSet
                {
                    Key = submission.reason
                },
                StartTime = submission.startDate.AddHours(submission.startTime),
                Visitors = new List<Contact>()
            };

            if (submission.visitors != null)
            {
                foreach (var visitor in submission.visitors)
                {
                    var match = Regex.Match(visitor.name.Trim(), @"^(?<first>[^\s]+)\s*(?<last>.*)$");

                    var firstName = match.Groups["first"].Value;
                    var lastName = match.Groups["last"].Value;

                    accessRequest.Visitors.Add(
                        new Contact
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            EmailAddress = visitor.email,
                            PhoneNumber = visitor.telephone
                        }
                    );
                }
            }

            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Chair, submission.chair));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Laptop, submission.laptop));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Tablet, submission.tablet));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Monitor, submission.monitor));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.DockingStation, submission.dockingStation));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Keyboard, submission.keyboard));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Mouse, submission.mouse));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Cables, submission.cables));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Headset, submission.headset));
            accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Printer, submission.printer));

            if (!string.IsNullOrWhiteSpace(submission.other))
            {
                accessRequest.AssetRequests.Add(new AssetRequest
                {
                    Asset = new OptionSet
                    {
                        Key = (int)Asset.Other
                    },
                    Other = submission.other
                });
            }

            await Http.PostAsJsonAsync("api/accessrequests/create", accessRequest);

            NavigationManager.NavigateTo("/access-requests");

            static IEnumerable<AssetRequest> Repeat(int value, int count)
                => Enumerable.Repeat(new AssetRequest
                    {
                        Asset = new OptionSet
                        {
                            Key = value
                        }
                    }, count);
        }
    }
}