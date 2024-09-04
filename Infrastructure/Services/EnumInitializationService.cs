using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.IRepository;
using Domain.Models.DbEnums;
using Domain.Contracts;

namespace ApplicationTemplate.Infrastructure.Services
{
    public class EnumInitializationService(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public void InitializeEnums()
        {
            using var scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            AddEnums<PlayResult>(dbContext);

            dbContext.SaveChanges();
        }

        public static IEnumerable<Type> GetDerivedTypes<T>()
        {
            var baseType = typeof(T);
            var assembly = baseType.Assembly;
            return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
        }

        private void AddEnums<TEnum>(IApplicationDbContext context) where TEnum : DbEnum
        {
            var dbSetProperty = context.GetType().GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(DbSet<TEnum>));

            if (dbSetProperty == null)
            {
                throw new InvalidOperationException($"DbSet<{typeof(TEnum).Name}> not found in DbContext.");
            }

            var dbSet = (DbSet<TEnum>)dbSetProperty.GetValue(context);

            foreach (var enumValue in DbEnum.GetStaticFields<TEnum>())
            {
                if (!dbSet.Any(e => e.Id == enumValue.Id))
                {
                    dbSet.Add((TEnum)enumValue);
                }
            }
        }
    }
}
