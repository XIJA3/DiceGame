using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationTemplate.Infrastructure.Data.Configurations
{
    public class AuthorizationLogConfiguration : IEntityTypeConfiguration<AuthorizationLog>
    {
        public void Configure(EntityTypeBuilder<AuthorizationLog> builder)
        {
            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.StartBalance)
                .HasColumnType("decimal(18, 2)") // Adjust precision and scale as needed
                .IsRequired();

            builder.Property(x => x.EndBalance)
                .HasColumnType("decimal(18, 2)") // Adjust precision and scale as needed
                .IsRequired();

            builder.HasOne(ce => ce.User)
                   .WithMany(p => p.AuthorizationLogs)
                   .HasForeignKey(ce => ce.UserId);
        }
    }

}
