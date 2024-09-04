using Models;

namespace DataTransferModels.ValueObjects
{
    public class Language(string language) : ValueObject
    {
        public static Language From(string langInitials)
        {
            var language = new Language(langInitials.ToLower());

            if (!SupportedLanguages.Contains(language))
            {
                throw new Exception();
            }

            return language;
        }


        public static Language KA => new("ka");
        public static Language EN => new("en");
        public static Language RU => new("ru");


        public string Name { get; private set; } = string.IsNullOrWhiteSpace(language) ? "NULL" : language;

        public static implicit operator string(Language colour)
        {
            return colour.ToString();
        }

        public static explicit operator Language(string code)
        {
            return From(code);
        }

        public override string ToString()
        {
            return Name;
        }

        protected static IEnumerable<Language> SupportedLanguages
        {
            get
            {
                yield return KA;
                yield return EN;
                yield return RU;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
