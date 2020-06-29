using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainUserService _domainUserService;

        public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService, IDomainUserService domainUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _domainUserService = domainUserService;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? string.Empty;
            string userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _domainUserService.GetUserNameAsync(AdAccount.For(userId));
            }

            _logger.LogInformation("OfficeEntry Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, userName, request);
        }
    }
}