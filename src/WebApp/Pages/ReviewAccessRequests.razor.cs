using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using OfficeEntry.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class ReviewAccessRequests : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IMediator Mediator { get; set; }

        private Domain.Entities.AccessRequest[] accessRequests;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            locale = (locale == Locale.French) ? locale : Locale.English;

            accessRequests = (await Mediator.Send(new GetManagerAccessRequestsQuery())).ToArray();

            foreach (var accessRequest in accessRequests)
            {
                accessRequest.Building.Name = (locale == Locale.French) ? accessRequest.Building.FrenchName : accessRequest.Building.EnglishName;
                accessRequest.Floor.Name = (locale == Locale.French) ? accessRequest.Floor.FrenchName : accessRequest.Floor.EnglishName;
            }

            StateHasChanged();

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
