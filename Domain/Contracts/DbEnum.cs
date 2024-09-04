using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Domain.Contracts
{
    public abstract class DbEnum(long id, string name) : ValueObject, IDbEnum
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public long Id { get; set; } = id;
        [Required]
        public string Name { get; set; } = name;


        public static DbEnum From(long id, string name, Type enumType)
        {
            var dbEnum = Activator.CreateInstance(enumType, id, name) as DbEnum;

            if (dbEnum is null || !dbEnum.SupportedDBEnums.Contains(dbEnum))
            {
                throw new Exception();
            }

            return dbEnum;
        }

        public static implicit operator string(DbEnum dbEnum)
        {
            return dbEnum.ToString();
        }


        public override string ToString()
        {
            return $"{Id}: {Name}";
        }

        public static T GetRepresentingDbEnum<T>(long id) where T : IDbEnum
        {
            var enumFields = GetStaticFields<T>();

            return enumFields.First(x => x.Id == id);
        }

        public static IEnumerable<T> GetStaticFields<T>() where T : IDbEnum
        {
            return typeof(T)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(T))
                .Select(f => (T)f.GetValue(null));
        }

        public virtual IEnumerable<IDbEnum> SupportedDBEnums
        {
            get
            {
                var type = GetType();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                 .Where(fi => fi.FieldType == type);

                foreach (var field in fields)
                {
                    yield return field.GetValue(null) as IDbEnum;
                }
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            //yield return Name;
        }
    }
}
