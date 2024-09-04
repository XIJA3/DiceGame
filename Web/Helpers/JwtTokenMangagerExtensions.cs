using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Web.Helpers.GlobalConfiguration;

namespace Web.Helpers
{
    internal static class JwtTokenManagerExtensions
    {
        private static readonly int tokenLife = 30;

        internal static List<string>? GetTokenClaim(string? token, string claimName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)) return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                var claimValue = securityToken.Claims.Where(c => c.Type == claimName).Select(x => x.Value).ToList();
                return claimValue;
            }
            catch (Exception)
            {
                return default;
            }
        }

        internal static string GenerateToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiratonTime = DateTime.UtcNow.AddMinutes(tokenLife);

            var token = new JwtSecurityToken(
                ConfigIssuer,
                ConfigAudience,
                claims,
                expires: expiratonTime,
                signingCredentials: credentials);

            var t = new JwtSecurityTokenHandler().WriteToken(token);

            return t;
        }


        internal static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        internal static void ValidateSecurityToken(SecurityToken securityToken)
        {
            var jwtToken = securityToken as JwtSecurityToken
                ?? throw new ArgumentException("The provided security token is not a JWT token.");

            if (jwtToken.ValidFrom > DateTime.UtcNow)
                throw new SecurityTokenException("Token is not yet valid.");

            if (jwtToken.ValidTo < DateTime.UtcNow)
                throw new SecurityTokenException("Token has expired.");

            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
        }

        internal static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = ConfigIssuer,
                ValidAudience = ConfigAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigKey))
            };
        }

        internal static string ConfigIssuer =>
            Configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Issuer is null");

        internal static string ConfigAudience =>
            Configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Audience is null");

        internal static string ConfigKey =>
            Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Key is null");
    }
}
