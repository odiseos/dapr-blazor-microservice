using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using User_API.Context;

namespace User.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UserController(UserDbContext context)
        {
            _context = context;

            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<User_API.Context.User>), (int)HttpStatusCode.OK)]
        public Task<List<User_API.Context.User>> Get() => _context.Users.ToListAsync();
    }
}
