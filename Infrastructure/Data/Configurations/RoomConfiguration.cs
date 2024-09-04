using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Infrastructure.Data.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);

            // Optional relationship to Session
            builder.HasOne(r => r.Session)
                   .WithOne(s => s.Room)
                   .HasForeignKey<Session>(s => s.RoomId)
                   .IsRequired(false); // Room can exist without a Session
        }
    }
}
