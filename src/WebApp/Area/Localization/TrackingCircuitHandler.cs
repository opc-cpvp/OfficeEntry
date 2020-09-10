using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Area.Localization
{
    /// <summary>
    /// Keep track of the Blazor connection tokens by their SignalR <see
    /// cref="Microsoft.AspNetCore.Components.Server.Circuits.Circuit.Id"/>. Used to clean-up the
    /// <see cref="UrlLocalizationAwareWebSocketsMiddleware._cultureByConnectionTokens"/> dictionary
    /// when a connection is closed.
    /// </summary>
    /// <remarks>
    /// The connection tokens and the circuit ids seem to be created together and they seem to be
    /// used to represent the websocket connections. Their appear to be "quantum entangled".
    /// </remarks>
    public class TrackingCircuitHandler : CircuitHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly ConcurrentDictionary<string, string> _connectionTokenByCircuit = new ConcurrentDictionary<string, string>();

        internal event EventHandler<CircuitHandlerConnectionDownEventArgs> ConnectionDown;

        public TrackingCircuitHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Save the circuit id and connection token "quantum entanglement".
        /// </summary>
        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            var components = QueryHelpers.ParseQuery(_httpContextAccessor.HttpContext.Request.QueryString.Value);
            var connectionToken = components["id"];
            _connectionTokenByCircuit[circuit.Id] = connectionToken;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Clean-up the circuit id and connection token "quantum entanglement".
        /// </summary>
        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            if (_connectionTokenByCircuit.TryGetValue(circuit.Id, out var connectionToken))
            {
                ConnectionDown?.Invoke(this, new CircuitHandlerConnectionDownEventArgs(connectionToken));
            }

            _connectionTokenByCircuit.TryRemove(circuit.Id, out var _);

            return Task.CompletedTask;
        }

        internal class CircuitHandlerConnectionDownEventArgs : EventArgs
        {
            public CircuitHandlerConnectionDownEventArgs(string connectionToken)
            {
                ConnectionToken = connectionToken;
            }

            public string ConnectionToken { get; }
        }
    }
}
