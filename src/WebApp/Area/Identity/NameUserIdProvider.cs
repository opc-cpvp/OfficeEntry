using IdentityModel;
using Microsoft.AspNetCore.SignalR;

namespace OfficeEntry.WebApp.Area.Identity
{
    /// <remarks>
    /// If Windows authentication is configured in your app, SignalR can use
    /// that identity to secure hubs. However, to send messages to individual
    /// users, you need to add a custom User ID provider. The Windows
    /// authentication system doesn't provide the "Name Identifier" claim.
    /// SignalR uses the claim to determine the user name.
    ///
    /// https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-3.1
    /// </remarks>
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var nameClaim = connection.User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name);

            return nameClaim?.Value;
        }
    }
}
