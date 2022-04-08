using Dapr;
using Microsoft.AspNetCore.Mvc;
using User.API.Message;
using User_API.IntegrationEvents;

namespace User.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";
        private const string DAPR_PUBSUB_TOPIC_NAME = "user-to-process";

        private readonly IUserToProcessEventHandler _handler;

        public IntegrationEventController(IUserToProcessEventHandler handler)
        {
            _handler = handler;
        }

        [HttpPost(DAPR_PUBSUB_TOPIC_NAME)]
        [Topic(DAPR_PUBSUB_NAME, DAPR_PUBSUB_TOPIC_NAME)]
        public Task HandleAsync(UserMessage message) => _handler.Handle(message);
    }
}
