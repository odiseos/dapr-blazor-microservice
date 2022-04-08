using User.API.Message;

namespace User_API.IntegrationEvents
{
    public interface IUserToProcessEventHandler
    {
        Task Handle(UserMessage message);
    }
}
