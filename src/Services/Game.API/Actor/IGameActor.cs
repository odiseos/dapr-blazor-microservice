using Dapr.Actors;
using Game_API.Message;

namespace Stoxapi_invoice.Business.Actors
{
    public interface IGameActor : IActor
    {
        Task ProcessUser(UserMessage user);
    }
}
