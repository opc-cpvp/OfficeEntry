using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.AccessRequest.Queries.GetAccessRequests
{
    public class GetAccessRequestsQuery : IRequest<IEnumerable<AccessRequestDto>>
    {
    }

    public class GetAccessRequestsQueryHandler : IRequestHandler<GetAccessRequestsQuery, IEnumerable<AccessRequestDto>>
    {
        public Task<IEnumerable<AccessRequestDto>> Handle(GetAccessRequestsQuery request, CancellationToken cancellationToken)
        {
            ////var rng = new Random();

            ////var vm = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            ////{
            ////    Date = DateTime.Now.AddDays(index),
            ////    TemperatureC = rng.Next(-20, 55),
            ////    Summary = Summaries[rng.Next(Summaries.Length)]
            ////});
            ///
            var vm = Enumerable.Range(1, 5).Select(Index => new AccessRequestDto
            {
                Name = "My Name",
                Location = "30 Victoria",
                EntryTime = "June 11th, 2020 - 09:00 to 13:00",
                Status = "Pending"
            });

            return Task.FromResult(vm);
        }
    }

    public class AccessRequestDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string EntryTime { get; set; }
        public string Status { get; set; }
    }
}
