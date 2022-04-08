using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInLine.Dapr.EventPubSub
{
    public class EventPubSub : IEventPubSub
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger _logger;
        private readonly string _pubSubName;

        public EventPubSub(DaprClient daprClient, ILogger<EventPubSub> logger, string pubSubName)
        {
            _daprClient = daprClient;
            _logger = logger;
            _pubSubName = pubSubName;
        }

        public async Task PublishAsync<T>(T message, string topicName)
        {
            _logger.LogInformation("Publishing event {@Event} to {PubsubName}.{TopicName}", message, _pubSubName, topicName);
            await _daprClient.PublishEventAsync(_pubSubName, topicName, message);
        }
    }
}
