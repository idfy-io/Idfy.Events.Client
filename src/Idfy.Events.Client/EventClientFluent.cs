﻿using System;
using System.Threading.Tasks;
using Idfy.Events.Entities;
using Idfy.Events.Entities;
using Rebus.Config;
using Rebus.Logging;

namespace Idfy.Events.Client
{
    public static class EventClientFluent
    {
        /// <summary>
        /// Plugin - a logger which is compatible with Rebus. Read more here: https://github.com/rebus-org/Rebus/wiki/Logging
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static EventClient UseRebusCompatibleLogger(this EventClient eventClient, object loggerFactory)
        {
            if (loggerFactory is IRebusLoggerFactory factory)
                eventClient.RebusLoggerFactory = factory;
            
            if(loggerFactory != null)
                eventClient.LogToConsole = false;
            
            return eventClient;
        }

        /// <summary>
        /// Sets up a console logger in Rebus. You can only have one logger, so do not combine this with another logger
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="logLevel"></param>
        /// <param name="logToConsole"></param>
        /// <returns></returns>
        public static EventClient LogToConsole(this EventClient eventClient, LogLevel logLevel = LogLevel.Debug, bool logToConsole = true)
        {
            var internalLogLevel = (Rebus.Logging.LogLevel)Enum.Parse(typeof(Rebus.Logging.LogLevel), logLevel.ToString());

            eventClient.LogToConsole = logToConsole;
            eventClient.LogLevel = internalLogLevel;
            
            if(logToConsole)
                eventClient.RebusLoggerFactory = null;
            
            return eventClient;
        }
        
        /// <summary>
        /// Subscribe to all raised events.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToAllEvents(this EventClient eventClient, Func<Event, Task> @event)
        {
            if (@event != null)
                eventClient.SubscribeToAllEvents(@event);
            return eventClient;
        }
        
        /// <summary>
        /// Subscribe to DocumentSignedEvent. This is raised when all the signers have signed a document.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentSignedEvent(this EventClient eventClient,
            Func<DocumentSignedEvent, Task> @event)
        {
            if(@event!=null)
                eventClient.SubscribeToDocumentSignedEvent(@event);
            return eventClient;
        }

        /// <summary>
        /// Subscribe to DocumentCanceledEvent. This is raised when a document is canceled either by the sender or the receiver.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentCanceledEvent(this EventClient eventClient,
           Func<DocumentCanceledEvent, Task> @event)
        {
            if(@event!=null)
                eventClient.SubscribeToDocumentCanceledEvent(@event);
            return eventClient;
        }

        /// <summary>
        /// Subscribe to DocumentSignedPartiallySignedEvent. This is raised when a document is signed by one of the signers.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentPartiallySignedEvent(this EventClient eventClient,
           Func<DocumentPartiallySignedEvent, Task> @event)
        {
            if(@event!=null)
            eventClient.SubscribeToDocumentPartiallySignedEvent(@event);
            return eventClient;
        }


        /// <summary>
        /// Subscribe to DocumentFormPartiallySignedEvent. This is raised when a form is signed by one of the signers.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentFormPartiallySignedEvent(this EventClient eventClient,
           Func<DocumentFormPartiallySignedEvent, Task> @event)
        {
            if (@event != null)
                eventClient.SubscribeToDocumentFormPartiallySignedEvent(@event);
            return eventClient;
        }

        /// <summary>
        /// Subscribe to DocumentFormSignedEvent. This is raised when a form is signed by all required signers.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentFormSignedEvent(this EventClient eventClient,
           Func<DocumentFormSignedEvent, Task> @event)
        {
            if (@event != null)
                eventClient.SubscribeToDocumentFormSignedEvent(@event);
            return eventClient;
        }

        /// <summary>
        /// Subscribe to DocumentCreatedEvent. This is raised when a new document is created.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentCreatedEvent(this EventClient eventClient,
            Func<DocumentCreatedEvent, Task> @event)
        {
            if (@event != null)
                eventClient.SubscribeToDocumentCreatedEvent(@event);
            return eventClient;
        }

        /// <summary>
        /// Subscribe to DocumentExpiredEvent. This is raised when a document expires.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentExpiredEvent(this EventClient eventClient, Func<DocumentExpiredEvent, Task> @event)
        {
            if (@event != null)
                eventClient.SubscribeToDocumentExpiredEvent(@event);
            return eventClient;
        }

        /// <summary>
        /// Subscribe to DocumentBeforeDeletedEvent. This is raised when a document is about to be deleted.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentBeforeDeletedEvent(this EventClient eventClient, Func<DocumentBeforeDeletedEvent, Task> @event)
        {
            if (@event != null)
                eventClient.SubscribeToDocumentBeforeDeletedEvent(@event);
            return eventClient;
        }

        /// <summary>
        /// Subscribe to the DocumentDeletedEvent. This is raised when a document is deleted.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventClient SubscribeToDocumentBeforeDeletedEvent(this EventClient eventClient, Func<DocumentDeletedEvent, Task> @event)
        {
            if (@event != null)
                eventClient.SubscribeToDocumentDeletedEvent(@event);
            return eventClient;
        }

        /// <summary>
        /// Start the event listener. It is important to call this function or the client will not listen to any events
        /// </summary>
        /// <param name="eventClient"></param>
        public static EventClient Start(this EventClient eventClient)
        {
            eventClient.Start();
            return eventClient;
        }

        /// <summary>
        /// Enables the use of a Rebus compatible logger using the RebusLogging configuration syntax, for example x.Serilog(new LoggerConfiguration().WriteTo.ColoredConsole().MinimumLevel.Debug()) 
        /// For more examples go to https://github.com/rebus-org/Rebus/wiki/Logging
        /// </summary>
        /// <param name="eventClient"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static EventClient AddRebusCompatibeLogger(this EventClient eventClient, Action<RebusLoggingConfigurer> config)
        {
            eventClient.AddRebusCompatibeLogger(config);
            return eventClient;
        }
    }
}