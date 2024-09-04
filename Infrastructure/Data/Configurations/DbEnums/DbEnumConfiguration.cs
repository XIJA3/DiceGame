using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AutoMapper.Execution;
using Domain.Contracts;


namespace ApplicationTemplate.Infrastructure.Data.Configurations.DbEnums
{
    public abstract class DbEnumConfiguration<TDbEnum> : IEntityTypeConfiguration<TDbEnum> where TDbEnum : DbEnum, IDbEnum
    {
        public void Configure(EntityTypeBuilder<TDbEnum> builder)
        {
            builder.HasKey(tt => tt.Id);

            builder.Property(tt => tt.Id)
                .ValueGeneratedNever(); // Disable automatic identity generation

            builder.Property(tt => tt.Name)
                .IsRequired()
                .HasMaxLength(50);

            AdditionalConfiguration(builder);
        }

        protected virtual void AdditionalConfiguration(EntityTypeBuilder<TDbEnum> builder) { }

        protected void ConfigureOneToMany<TChild>(
        EntityTypeBuilder<TDbEnum> builder,
        Expression<Func<TDbEnum, IEnumerable<TChild>?>> navigationProperty,
        Expression<Func<TChild, TDbEnum?>> inverseNavigationProperty)
        where TChild : class
        {
            builder.HasMany(navigationProperty)
                   .WithOne(inverseNavigationProperty)
                   .HasForeignKey($"{inverseNavigationProperty.GetMember().Name}Id")
                   .HasPrincipalKey(a => a.Id)
                   .OnDelete(DeleteBehavior.Restrict); // or NoAction, Cascade, etc.
        }

        // Todo: To Test 
        protected void ConfigureManyToMany<TChild>(
        EntityTypeBuilder<TDbEnum> builder,
        Expression<Func<TDbEnum, IEnumerable<TChild>?>> navigationProperty,
        Expression<Func<TChild, IEnumerable<TDbEnum>?>> inverseNavigationProperty)
        where TChild : class
        {
            builder.HasMany(navigationProperty)
                   .WithMany(inverseNavigationProperty);
        }
    }
}
