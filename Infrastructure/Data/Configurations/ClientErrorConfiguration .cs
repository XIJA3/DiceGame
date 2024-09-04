using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationTemplate.Infrastructure.Data.Configurations
{
    public class ClientErrorConfiguration : IEntityTypeConfiguration<ClientError>
    {
        public void Configure(EntityTypeBuilder<ClientError> builder)
        {
            builder.Property(x => x.UserId)
                .IsRequired();

            // Configure the foreign key relationship with User
            builder.HasOne(ce => ce.User)
                   .WithMany(p => p.ClientErrors)
                   .HasForeignKey(ce => ce.UserId);
                   //.OnDelete(DeleteBehavior.Cascade); // Optional: Specify delete behavior
        }
    }
}
