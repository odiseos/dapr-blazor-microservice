using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User.API.Context
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User_API.Context.User>
    {
        public void Configure(EntityTypeBuilder<User_API.Context.User> orderConfiguration)
        {
            orderConfiguration.ToTable("User");

            orderConfiguration.HasKey(o => o.Id);
        }
    }
}
