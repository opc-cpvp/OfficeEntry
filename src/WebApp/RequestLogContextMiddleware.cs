using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace OfficeEntry.WebApp;

public static class RequestLogContextMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLogContextMiddleware>();
    }
}

public class RequestLogContextMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLogContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        context.Request.Headers.TryGetValue("Cko-Correlation-Id", out StringValues ckoId);
        var correlationId = ckoId.FirstOrDefault() ?? context.TraceIdentifier;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            return _next.Invoke(context);
        }
    }
}
