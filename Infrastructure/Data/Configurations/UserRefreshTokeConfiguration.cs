using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationTemplate.Infrastructure.Data.Configurations;

public class UserRefreshTokeConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.Property(t => t.UserName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.RefreshToken)
            .HasMaxLength(200)
            .IsRequired();
    }
}
