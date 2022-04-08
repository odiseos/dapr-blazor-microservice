using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Game_API.Context
{
    public class PlayedGameEntityTypeConfiguration : IEntityTypeConfiguration<PlayedGame>
    {
        public void Configure(EntityTypeBuilder<PlayedGame> orderConfiguration)
        {
            orderConfiguration.ToTable("PlayedGame");

            orderConfiguration.HasKey(o => o.Id);
        }
    }
}
