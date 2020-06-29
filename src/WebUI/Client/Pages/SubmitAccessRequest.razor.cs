﻿using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using OfficeEntry.Domain.Entities;
using OfficeEntry.WebUI.Client.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Client.Pages
{
    public abstract class SubmitAccessRequestBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            var submission = JsonConvert.DeserializeObject<AccessRequestSubmission>(surveyResult);

            var accessRequest = new Domain.Entities.AccessRequest
            {
                Building = new Building { Id = submission.building },
                Details = submission.details,
                EndTime = submission.startDate.AddHours(submission.endTime),
                Floor = new Floor { Id = submission.floor },
                Manager = new Contact { Id = submission.manager },
                Reason = new OptionSet
                {
                    Key = submission.reason
                },
                StartTime = submission.startDate.AddHours(submission.startTime)
            };

            accessRequest.Visitors = new List<Contact>();

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

            await Http.PostAsJsonAsync("api/accessrequests/create", accessRequest);

            NavigationManager.NavigateTo("/access-requests");
        }
    }
}