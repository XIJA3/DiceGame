using Microsoft.AspNetCore.Authentication;

namespace Web.Services
{
    public class MyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public static string DefaultScheme = "Authorization";
    }
}
