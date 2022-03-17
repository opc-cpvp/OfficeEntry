using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace OfficeEntry.Application.Common.Behaviours;

public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly IAsyncPolicy _retryPolicy;

    public RetryBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;

        _retryPolicy = Policy
            .Handle<ObjectDisposedException>(e => e.ObjectName is "System.Net.Http.HttpClient")
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(i), (exception, timeSpan) =>
            {
                // Logic to be executed before each retry, such as logging
                _logger.LogError(exception, "Waiting {timeSpan} seconds before retrying.", timeSpan);
            });
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        return _retryPolicy.ExecuteAsync(() => next());
    }
}
