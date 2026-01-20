using MagicOnion;
using MagicOnion.Server;
using Microsoft.Extensions.Logging;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Services;

/// <summary>
/// Implementation of the workspace spying service
/// </summary>
public sealed class RecordSpyingEventService : ServiceBase<IRecordSpyingEventService>, IRecordSpyingEventService
{
    private readonly ILogger<RecordSpyingEventService> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainUserService _domainUserService;


    public RecordSpyingEventService(ILogger<RecordSpyingEventService> logger, ICurrentUserService currentUserService, IDomainUserService domainUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _domainUserService = domainUserService;
    }

    /// <inheritdoc />
    public async UnaryResult HandleAsync(SpyingEventArg @event, DateOnly selectedDate)
    {
        @event.UserId = _currentUserService.UserId;
        @event.FullName = await _domainUserService.GetUserNameAsync(OfficeEntry.Domain.ValueObjects.AdAccount.For(_currentUserService.UserId));


        _logger.LogInformation("OfficeEntry UserSpying: {UserName} {Name} {Date} {Workspace} {Victim}",
            @event.UserId,
            @event.FullName,
            selectedDate.ToString("yyyy-MM-dd"),
            @event.Workspace,
            @event.Victim);
    }
}