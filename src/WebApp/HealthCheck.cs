using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OfficeEntry.Application.Locations.Queries.GetBuildings;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp
{
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
                var buildings = await _mediator.Send(new GetBuildingsQuery { Locale = "en" });

                if (buildings is { })
                {
                    return HealthCheckResult.Healthy("A healthy result.");
                }
                else
                {
                    _logger.LogError("No buildings found.");

                    return new HealthCheckResult(
                        context.Registration.FailureStatus,
                        "No buildings found.");
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
}
