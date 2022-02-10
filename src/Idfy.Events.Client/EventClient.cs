using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Idfy.Events.Client.Infastructure;
using Idfy.Events.Client.Infastructure.Bus;
using Idfy.Events.Entities;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Compression;
using Rebus.Config;
using Rebus.Encryption;
using Rebus.Logging;

namespace Idfy.Events.Client
{
    /// <summary>
    /// Event client that lets you subscribe to events that occurs on your account.
    /// Remember to dispose of the client when your program exits by calling the Dispose() method.
    /// </summary>
    public class EventClient : IDisposable
    {
        private const string Scope = "event";

        private readonly BuiltinHandlerActivator _adapter;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tokenUrl;
        private readonly string _accountId;

        private IBus _bus;
        private Action<RebusLoggingConfigurer> _rebusLoggingConfigurer;
        private bool _noRebusLogger;



        /// <summary>
        /// Sets up the event client to subscribe to events that occurs on the provided account.
        /// </summary>
        /// <param name="oauthClientId">Your OAuth client ID</param>
        /// <param name="oauthClientSecret">Your OAuth client secret</param>
        /// <param name="oauthServerUrl">The url to the oauth2 server (only for dev testing)</param>
        /// <param name="accountId">For certain clients the AccountID must be explicitly specified</param>
        /// <returns><see cref="EventClient"/></returns>
        public static EventClient Setup(string oauthClientId, string oauthClientSecret, string oauthServerUrl = null, string accountId = null)
        {
            if (string.IsNullOrWhiteSpace(oauthClientId))
                throw new ArgumentNullException(nameof(oauthClientId));

            if (string.IsNullOrWhiteSpace(oauthClientSecret))
                throw new ArgumentNullException(nameof(oauthClientSecret));

            var tokenUrl = string.IsNullOrEmpty(oauthServerUrl)
                ? null
                : $"{oauthServerUrl.EnsureEndsWithSlash()}connect/token";

            var adapter = new BuiltinHandlerActivator();
            return new EventClient(adapter, oauthClientId, oauthClientSecret, tokenUrl, accountId);
        }

        /// <summary>
        /// Disposes the Event Client, which also disposes the bus. You should always call this method when your program has completed.
        /// </summary>
        public void Dispose()
        {
            _bus?.Dispose();
        }

        internal bool LogToConsole { get; set; }

        internal LogLevel? LogLevel { get; set; }

        internal IRebusLoggerFactory RebusLoggerFactory { get; set; }

        internal RebusLoggingConfigurer Configurer { get; set; }

        internal void Subscribe<T>(Func<T, Task> func) where T : Event
        {
            _adapter.Handle(func);
        }

        internal void SubscribeToAllEvents(Func<Event, Task> func)
        {
            _adapter.Handle(func);
        }

        internal void Start()
        {
            _bus = ConfigureRebus().Start();
        }

        internal void AddRebusCompatibeLogger(Action<RebusLoggingConfigurer> config)
        {
            _rebusLoggingConfigurer = config;
            _noRebusLogger = config == null;
        }

        private EventClient(BuiltinHandlerActivator adapter, string clientId, string clientSecret, string tokenUrl = null, string accountId = null)
        {
            _adapter = adapter;
            _adapter.Handle<object>(InternalHandler);

            _noRebusLogger = true;
            _clientId = clientId;
            _clientSecret = WebUtility.UrlEncode(clientSecret);
            _tokenUrl = tokenUrl;
            _accountId = accountId;
        }



        private RebusConfigurer ConfigureRebus()
        {
            var config = GetEventClientConfiguration();

            return Configure.With(_adapter)
                .Transport(x => x.UseAzureServiceBus(config.ConnectionString, config.QueueName).DoNotCreateQueues().DoNotCheckQueueConfiguration())
                .Options(c =>
                {
                    c.AddNamespaceFilter();
                    c.EnableCompression();
                    c.EnableEncryption(config.EncryptionKey);
                })
                .Logging(x =>
                {
                    if (LogToConsole)
                        x.ColoredConsole(LogLevel ?? Rebus.Logging.LogLevel.Error);
                    else if (RebusLoggerFactory != null)
                    {
                        x.Use(RebusLoggerFactory);
                    }
                    else if (_rebusLoggingConfigurer != null)
                    {
                        _rebusLoggingConfigurer(x);
                    }
                    else if (_noRebusLogger)
                    {
                        x.None();
                    }
                });
        }

        private EventClientConfiguration GetEventClientConfiguration()
        {
            // Get access token
            var formData = new NameValueCollection()
            {
                {"grant_type", "client_credentials"},
                {"scope", Scope},
                {"client_id", _clientId},
                {"client_secret", _clientSecret}
            };

            var tokenResponse = Mapper<TokenResponse>.MapFromJson(Requestor.PostFormData(string.IsNullOrWhiteSpace(_tokenUrl) ? Urls.TokenEndpoint:_tokenUrl, formData));

            // Get event configuration
            var eventConfigUrl = $"{Urls.NotificationEndpoint}/client";

            var queryString = "?v2=true";

            if (!string.IsNullOrEmpty(_accountId))
            {
                queryString += $"&accountId={WebUtility.UrlEncode(_accountId)}";
            }

            var eventConfigResponse = Mapper<EventClientConfiguration>.MapFromJson(Requestor.GetString($"{eventConfigUrl}{queryString}", tokenResponse.AccessToken));

            if (string.IsNullOrWhiteSpace(eventConfigResponse.ConnectionString))
            {
                // first-time setup of client is required
                eventConfigResponse = Mapper<EventClientConfiguration>.MapFromJson(Requestor.PostString($"{eventConfigUrl}/setup{queryString}", token: tokenResponse.AccessToken));
            }

            return eventConfigResponse;
        }

        /// <summary>
        /// Internal handler for all message types. Prevents exceptions for events with no registered handler.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static Task InternalHandler(object msg)
        {
            return Task.FromResult(0);
        }
    }
}
