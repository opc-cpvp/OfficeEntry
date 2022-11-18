using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OfficeEntry.Application.Users.Queries.GetContactsRequests;

namespace OfficeEntry.WebApp;

public class HealthCheck : IHealthCheck
{
    private readonly IMediator _mediator;
    private readonly ILogger<HealthCheck> _logger;

    public HealthCheck(IMediator mediator, ILogger<HealthCheck> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var contacts = await _mediator.Send(GetContactsQuery.Instance, cancellationToken);

            if (contacts is { })
            {
                return HealthCheckResult.Healthy("A healthy result.");
            }
            else
            {
                _logger.LogError("No contacts found.");

                return new HealthCheckResult(
                    context.Registration.FailureStatus,
                    "No contacts found.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "health check - An unhealthy result");

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                "An unhealthy result.");
        }
    }
}
