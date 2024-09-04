using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Infrastructure.Data.Configurations
{
    public class PlayerConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.UserName)
                .IsRequired();

            builder.Property(x => x.Id)
                .ValueGeneratedNever(); // Disable automatic identity generation

            builder.HasMany(x => x.AuthorizationLogs)
                .WithOne()
                .HasForeignKey(x => x.UserId);


            builder.HasMany(x => x.ClientErrors)
                .WithOne()
                .HasForeignKey(x => x.UserId);
        }
    }
}
