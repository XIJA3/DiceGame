using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferModels.ValueObjects
{
    public class TokenClaim(string TokenClaim) : ValueObject
    {
        public static TokenClaim From(string claimName)
        {
            var tokenClaim = new TokenClaim(claimName);

            if (!SupportedTokenClaims.Contains(tokenClaim))
            {
                throw new Exception();
            }

            return tokenClaim;
        }


        public static TokenClaim UserName => new(nameof(UserName));
        public static TokenClaim UserId => new(nameof(UserId));
        public static TokenClaim RefreshToken => new(nameof(RefreshToken));
        public static TokenClaim SessionId => new(nameof(SessionId));
        public static TokenClaim Channel => new(nameof(Channel));


        public string Name { get; private set; } = string.IsNullOrWhiteSpace(TokenClaim) ? "NULL" : TokenClaim;

        public static implicit operator string(TokenClaim colour)
        {
            return colour.ToString();
        }

        public static explicit operator TokenClaim(string code)
        {
            return From(code);
        }

        public override string ToString()
        {
            return Name;
        }

        protected static IEnumerable<TokenClaim> SupportedTokenClaims
        {
            get
            {
                yield return UserName;
                yield return UserId;
                yield return RefreshToken;
                yield return SessionId;
                yield return Channel;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
