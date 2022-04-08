using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInLine.Dapr.EventPubSub
{
    public class EventPubSubProvider : IEventPubSubProvider
    {
        private readonly DaprClient _daprClient;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, IEventPubSub> _eventPubSubs;

        public EventPubSubProvider(DaprClient daprClient, ILoggerFactory loggerFactory)
        {
            _daprClient = daprClient;
            _loggerFactory = loggerFactory;
            _eventPubSubs = new ConcurrentDictionary<string, IEventPubSub>();
        }

        public IEventPubSub CreateEventPubSub(string pubsubName)
        {
            return GetEventPubSub(pubsubName) ?? MakeEventPubSub(pubsubName);
        }

        private IEventPubSub MakeEventPubSub(string pubsubName)
        {
            //create repository
            var storageStateService = new EventPubSub(_daprClient, _loggerFactory.CreateLogger<EventPubSub>(), pubsubName);

            //insert repository in dictionary
            _eventPubSubs[pubsubName] = storageStateService;

            return storageStateService;
        }

        private IEventPubSub? GetEventPubSub(string pubsubName)
        {
            if (_eventPubSubs.TryGetValue(pubsubName, out IEventPubSub? eventPubSub))
                return eventPubSub;

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_eventPubSubs != null)
                {
                    _eventPubSubs.Clear();
                }
            }
            _disposed = true;
        }
    }
}
