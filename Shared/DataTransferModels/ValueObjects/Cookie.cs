using Models;

namespace DataTransferModels.ValueObjects
{
    public class Cookie(string cookie) : ValueObject
    {
        public static Cookie From(string cookie)
        {
            var Cookie = new Cookie(cookie);

            if (!SupportedCookies.Contains(Cookie))
            {
                throw new Exception();
            }

            return Cookie;
        }


        public static Cookie Language => new("Language");
        public static Cookie AccessToken=> new("AccessToken");


        public string Name { get; private set; } = string.IsNullOrWhiteSpace(cookie) ? "NULL" : cookie;

        public static implicit operator string(Cookie colour)
        {
            return colour.ToString();
        }

        public static explicit operator Cookie(string code)
        {
            return From(code);
        }

        public override string ToString()
        {
            return Name;
        }

        protected static IEnumerable<Cookie> SupportedCookies
        {
            get
            {
                yield return Language;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
