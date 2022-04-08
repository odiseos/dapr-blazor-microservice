using Dapr.Actors.Runtime;
using FiveInLine.Dapr.EventPubSub;
using Game_API.Message;
using Newtonsoft.Json;

namespace Stoxapi_invoice.Business.Actors
{
    public class GameActor : Actor, IGameActor
    {  
        private readonly IEventPubSubProvider _eventPubSubProvider;

        private IEventPubSub EventPubSub => _eventPubSubProvider.CreateEventPubSub("pubsub");

        public GameActor(ActorHost host,
                         IEventPubSubProvider eventPubSubProvider) : base(host)
        {
            _eventPubSubProvider = eventPubSubProvider; 
        }

        public async Task ProcessUser(UserMessage user)
        {
            await EventPubSub.PublishAsync(user, "user-to-process");
        }
    }
}
