using Domain.Models.DbEnums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ApplicationTemplate.Infrastructure.Data.Configurations.DbEnums
{
    public class AccountTypeConfiguration : DbEnumConfiguration<AccountType>
    {
        protected override void AdditionalConfiguration(EntityTypeBuilder<AccountType> builder)
        {
            //ConfigureOneToMany(builder,
            //    tt => tt.FromTransactions,
            //    t => t.FromAccount);

            //ConfigureOneToMany(builder,
            //    tt => tt.ToTransactions,
            //    t => t.ToAccount);
        }
    }
}
