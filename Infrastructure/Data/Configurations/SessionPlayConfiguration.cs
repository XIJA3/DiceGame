using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Infrastructure.Data.Configurations
{
    public class SessionPlayConfiguration : IEntityTypeConfiguration<SessionPlay>
    {
        public void Configure(EntityTypeBuilder<SessionPlay> builder)
        {
            // Configure primary key
            builder.HasKey(sp => sp.Id);

            // Configure properties
            builder.Property(sp => sp.IsFinished)
                .IsRequired();

            builder.Property(sp => sp.SessionId)
                .IsRequired();

            // Configure relationships with UserInfo
            builder.HasMany(sp => sp.UserInfos)
                   .WithOne(ui => ui.SessionPlay)
                   .HasForeignKey(ui => ui.SessionPlayId)
                   .OnDelete(DeleteBehavior.Cascade); // Configure cascade delete

            // Configure relationship with Session
            builder.HasOne(sp => sp.Session)
                   .WithMany(s => s.Plays)
                   .HasForeignKey(sp => sp.SessionId)
                   .OnDelete(DeleteBehavior.Cascade); // Configure cascade delete


            // Additional configurations (if any) can be added here
        }
    }

}
