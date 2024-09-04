using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Infrastructure.Data.Configurations
{
    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.HasKey(ui => ui.Id);

            builder.Property(ui => ui.Score)
                   .IsRequired();

            builder.Property(ui => ui.UserId)
                   .IsRequired();

            builder.Property(ui => ui.PlayResultId)
                   .IsRequired(); // Ensure foreign key is required

            builder.Property(ui => ui.SessionPlayId)
                   .IsRequired();

            // Configure relationship with User
            builder.HasOne(ui => ui.User)
                   .WithMany() // Assuming User does not have a collection of UserInfos
                   .HasForeignKey(ui => ui.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with SessionPlay
            builder.HasOne(ui => ui.SessionPlay)
                   .WithMany(sp => sp.UserInfos)
                   .HasForeignKey(ui => ui.SessionPlayId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
