using User.API.Message;
using User_API.Context;

namespace User_API.IntegrationEvents
{
    public class UserToProcessEventHandler : IUserToProcessEventHandler
    {
        private readonly UserDbContext _context;

        public UserToProcessEventHandler(UserDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UserMessage message)
        {
            if (!_context.Users.Any(u => u.UserName == message.UserName))
            {
                _context.Users.Add(new Context.User { UserName = message.UserName });
                await _context.SaveChangesAsync();
            }
        }
    }
}
