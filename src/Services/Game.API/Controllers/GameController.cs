using Dapr.Actors;
using Dapr.Actors.Client;
using FiveInLine.Dapr.EventPubSub;
using FiveInLine.Dapr.Services;
using Game_API.Context;
using Game_API.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stoxapi_invoice.Business.Actors;
using System.Net;

namespace Game_API.Controllers
{
    [Route("api/v1/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IEventPubSubProvider _eventPubSubProvider;

        private IEventPubSub EventPubSub => _eventPubSubProvider.CreateEventPubSub("pubsub");

        public GameController(GameDbContext context, IActorProxyFactory actorProxyFactory, IEventPubSubProvider eventPubSubProvider)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _actorProxyFactory = actorProxyFactory;
            _eventPubSubProvider = eventPubSubProvider;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PlayedGame>), (int)HttpStatusCode.OK)]
        public Task<List<PlayedGame>> Get() => _context.PlayedGames.OrderByDescending(x => x.Points).ToListAsync();

        [HttpPost]
        public async Task Post([FromBody]GameMessage message) 
        {
            _context.PlayedGames.Add(new PlayedGame { Points = message.Points, DatePlayed = DateTime.Now, UserName = message.UserName });
            await _context.SaveChangesAsync();

            //await EventPubSub.PublishAsync(new UserMessage { UserName = message.UserName }, "user-to-process");

            //Create an Actor and call it to publish a message in the pubsub
            var actor = GetGameActor();
            await actor.ProcessUser(new UserMessage { UserName = message.UserName });
        }

        private IGameActor GetGameActor()
        {
            var actorId = new ActorId(Guid.NewGuid().ToString());
            return _actorProxyFactory.CreateActorProxy<IGameActor>(actorId, nameof(GameActor));
        }
    }
}
