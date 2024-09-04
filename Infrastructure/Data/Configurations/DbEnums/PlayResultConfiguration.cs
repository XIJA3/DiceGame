using Domain.Models.DbEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ApplicationTemplate.Infrastructure.Data.Configurations.DbEnums
{
    public class PlayResultConfiguration : DbEnumConfiguration<PlayResult>
    {
        protected override void AdditionalConfiguration(EntityTypeBuilder<PlayResult> builder)
        {
            //ConfigureOneToMany(builder,
            //    tt => tt.UserInfos,
            //    t => t.PlayResult);
        }
    }
}
