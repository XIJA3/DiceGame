using System.Security.Claims;

namespace Web.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static SerializableClaimsPrincipal ToSerializableClaimsPrincipal(this ClaimsPrincipal principal)
        {
            return new SerializableClaimsPrincipal
            {
                Identities = principal.Identities.Select(i => new SerializableClaimsIdentity
                {
                    AuthenticationType = i.AuthenticationType,
                    NameClaimType = i.NameClaimType,
                    RoleClaimType = i.RoleClaimType,
                    Claims = i.Claims.Select(c => new SerializableClaim
                    {
                        Type = c.Type,
                        Value = c.Value,
                        ValueType = c.ValueType,
                        Issuer = c.Issuer,
                        OriginalIssuer = c.OriginalIssuer
                    }).ToList()
                }).ToList()
            };
        }

        public static ClaimsPrincipal ToClaimsPrincipal(this SerializableClaimsPrincipal principal)
        {
            return new ClaimsPrincipal(principal.Identities.Select(i => new ClaimsIdentity(
                i.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType, c.Issuer, c.OriginalIssuer)),
                i.AuthenticationType,
                i.NameClaimType,
                i.RoleClaimType
            )));
        }
    }

    public class SerializableClaimsPrincipal
    {
        public List<SerializableClaimsIdentity> Identities { get; set; } = new List<SerializableClaimsIdentity>();
    }

    public class SerializableClaimsIdentity
    {
        public string? AuthenticationType { get; set; }
        public string? NameClaimType { get; set; }
        public string? RoleClaimType { get; set; }
        public List<SerializableClaim> Claims { get; set; } = new List<SerializableClaim>();
    }

    public class SerializableClaim
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string ValueType { get; set; } = ClaimValueTypes.String;
        public string Issuer { get; set; } = ClaimsIdentity.DefaultIssuer;
        public string OriginalIssuer { get; set; } = ClaimsIdentity.DefaultIssuer;
    }
}