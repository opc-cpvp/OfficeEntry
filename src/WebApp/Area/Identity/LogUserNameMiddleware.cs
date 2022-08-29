using Serilog.Context;

namespace OfficeEntry.WebApp.Area.Identity;

public class LogUserNameMiddleware
{
    private readonly RequestDelegate next;

    public LogUserNameMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var userName = context?.User?.Claims
            ?.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)
            ?.Value ?? "anonymous";

        LogContext.PushProperty("UserName", userName);

        return next(context);
    }
}
