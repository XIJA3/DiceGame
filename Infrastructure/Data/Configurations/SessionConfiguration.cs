using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationTemplate.Infrastructure.Data.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(s => s.Id);

            // Required relationship to Room
            builder.HasOne(s => s.Room)
                   .WithOne() // No inverse navigation in Room
                   .HasForeignKey<Session>(s => s.RoomId)
                   .IsRequired(); // Session must have a Room
        }
    }
}
