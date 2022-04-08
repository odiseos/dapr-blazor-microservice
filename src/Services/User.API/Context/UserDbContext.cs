using Microsoft.EntityFrameworkCore;
using User.API.Context;

namespace User_API.Context
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }
}
