using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Users.Queries.GetContactsRequests
{
    public class GetContactsQuery : IRequest<IEnumerable<Domain.Entities.Contact>>
    {
    }

    public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, IEnumerable<Domain.Entities.Contact>>
    {
        private readonly IUserService _userService;

        public GetContactsQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IEnumerable<Domain.Entities.Contact>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
        {
            var result = await _userService.GetContacts();

            // TODO: what should we do with the result
            if (!result.Result.Succeeded)
            {
            }

            return result.Contacts;
        }
    }
}