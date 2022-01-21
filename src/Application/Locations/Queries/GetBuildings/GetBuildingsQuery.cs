using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Locations.Queries.GetBuildings;

public class GetBuildingsQuery : IRequest<IEnumerable<Building>>
{
    public string Locale { get; set; }
}

public class GetBuildingsQueryHandler : IRequestHandler<GetBuildingsQuery, IEnumerable<Building>>
{
    private readonly ILocationService _locationService;

    public GetBuildingsQueryHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public Task<IEnumerable<Building>> Handle(GetBuildingsQuery request, CancellationToken cancellationToken)
    {
        return _locationService.GetBuildingsAsync(request.Locale);
    }
}
