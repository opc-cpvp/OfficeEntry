using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Area.Localization
{
    /// <remarks>
    /// Microsoft recommends the use of a cookie to ensures that the WebSocket connection can
    /// correctly propagate the culture.
    ///
    ///     If localization schemes are based on the URL path or query string, the scheme might not
    ///     be able to work with WebSockets, thus fail to persist the culture.
    ///
    ///     Therefore, use of a localization culture cookie is the recommended approach.
    ///
    /// This is not the approach we want as people can have multiple browser screens (browser tab or
    /// iframe) using different languages.
    /// </remarks>
    public class UrlLocalizationAwareWebSocketsMiddleware
    {
        /*
         * Localization Extensibility
         * https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization-extensibility?view=aspnetcore-3.1
         *
         * Write custom ASP.NET Core middleware
         * https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-3.1
         *
         *
         * https://docs.microsoft.com/en-us/aspnet/core/blazor/advanced-scenarios?view=aspnetcore-3.1#blazor-server-circuit-handler
         */

        private static readonly ConcurrentDictionary<string, string> _cultureByConnectionTokens = new ConcurrentDictionary<string, string>();
        private static bool _cleanUpRegistered = false;
        private static readonly object _lock = new object();
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlLocalizationAwareWebSocketsMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The delegate representing the remaining middleware in the request pipeline.
        /// </param>
        public UrlLocalizationAwareWebSocketsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// When "closing" the SignalR connection (websocket) clean-up the memory by removing the
        /// token from the dictionary.
        /// </summary>
        private static void CleanUpRegistration(CircuitHandler trackingCircuitHandler)
        {
            if (_cleanUpRegistered)
            {
                return;
            }

            lock (_lock)
            {
                var handler = (TrackingCircuitHandler)trackingCircuitHandler;

                handler.ConnectionDown += (s, e) => _cultureByConnectionTokens.TryRemove(e.ConnectionToken, out _);

                _cleanUpRegistered = true;
            }
        }

        /// <summary>
        /// Handles the requests and returns a Task that represents the execution of the middleware.
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context reprensenting the request.
        ///
        /// NOTE: This parameter is dependency resolved by the service provider using:
        ///     services.AddSingleton<Microsoft.AspNetCore.Components.Server.Circuits.CircuitHandler>
        /// </param>
        /// <param name="trackingCircuitHandler">
        /// The SignalR circuit handler.
        ///
        /// NOTE: This parameter is dependency resolved by the service provider using:
        ///     services.AddHttpContextAccessor()
        /// </param>
        public async Task InvokeAsync(HttpContext httpContext, CircuitHandler trackingCircuitHandler)
        {
            CleanUpRegistration(trackingCircuitHandler);

            var segments = httpContext.Request.Path.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            var nextAction = segments switch
            {
                string[] { Length: 2 } x when x[0] == "_blazor" && x[1] == "negotiate"                      => BlazorNegotiate,
                string[] { Length: 1 } x when x[0] == "_blazor" && httpContext.Request.QueryString.HasValue => BlazorHeartbeat,
                _ => _next
            };

            await nextAction(httpContext);
        }

        /// <summary>
        /// On blazor heartbeat, set the culture based on the dictionary entry created in
        /// <see cref="BlazorNegotiate(HttpContext httpContext)"/>.
        /// </summary>
        private async Task BlazorHeartbeat(HttpContext httpContext)
        {
            var components = QueryHelpers.ParseQuery(httpContext.Request.QueryString.Value);
            var connectionToken = components["id"];
            var currentCulture = _cultureByConnectionTokens[connectionToken];

            var culture = new CultureInfo(currentCulture);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await _next(httpContext);
        }

        /// <summary>
        /// On blazor negotiate, set the culture based on the referer and save it to a
        /// dictionary to be used by the Blazor heartbeat
        /// <see cref="BlazorHeartbeat(HttpContext)"/>.
        /// </summary>
        private async Task BlazorNegotiate(HttpContext httpContext)
        {
            var currentCulture = GetCultureFromReferer(httpContext);

            // Set the culture
            var culture = new CultureInfo(currentCulture);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            httpContext.Request.EnableBuffering();

            var originalResponseBodyStream = httpContext.Response.Body;
            using var responseBody = new MemoryStream();
            httpContext.Response.Body = responseBody;

            await _next(httpContext);

            // Temporary unwarp the response body to get the connectionToken
            var responseBodyContent = await ReadResponseBodyAsync(httpContext.Response);

            if (httpContext.Response.ContentType == "application/json")
            {
                var root = JsonSerializer
                    .Deserialize<BlazorNegociateBody>(responseBodyContent, BlazorNegociateBody.JsonSerializerOptions);
                _cultureByConnectionTokens[root.ConnectionToken] = currentCulture;
            }

            // Rewrap the response body as if we hadn't upwrap-it
            await responseBody.CopyToAsync(originalResponseBodyStream);
        }

        private static string GetCultureFromReferer(HttpContext httpContext)
        {
            var referer = httpContext.Request.Headers["Referer"].ToString();
            var uri = new Uri(referer);

            var refererSegments = uri.LocalPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (refererSegments.Length > 1 && refererSegments[0].Length == 2)
            {
                var culture = refererSegments[0];
                return culture;
            }

            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        }

        private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(response.Body, leaveOpen: true);
            var bodyAsText = await reader.ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private class BlazorNegociateBody
        {
            public int NegotiateVersion { get; set; }
            public string ConnectionToken { get; set; }

            public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }
    }
}
