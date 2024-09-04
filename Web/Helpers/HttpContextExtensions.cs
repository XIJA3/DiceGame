using DataTransferModels.ValueObjects;
using Microsoft.Extensions.Primitives;
using System.Web;
using Web.Exceptions;
using Web.Helpers;
using Web.Services;

namespace Web.Helpers
{
    public static class HttpContextExtensions
    {
        //public static T GetValueFromQueryString<T>(this IHttpContextAccessor httpContextAccessor, string key)
        //{
        //    if (httpContextAccessor?.HttpContext?.Request?.QueryString.HasValue == true)
        //    {
        //        var Value = HttpUtility.ParseQueryString(httpContextAccessor?.HttpContext?.Request?.QueryString.Value!).Get(key) ?? throw new CookieNotFound();

        //        var decrypted = CryptoService.Decrypt(Value);

        //        return decrypted.Cast<T>();
        //    }

        //    throw new CookieNotFound();
        //}

        public static T Cast<T>(this object variable)
        {
            return (T)variable;
        }

        public static void DeleteCookie(this HttpContext context, Cookie cookie)
        {
            //context.Response.Headers.Remove(MyAuthenticationOptions.DefaultScheme);
            context.Response.Cookies.Delete(cookie.Name);
        }

        public static void WriteCookie(this HttpContext context, Cookie cookie, string accessToken)
        {
            context.DeleteCookie(cookie);

            context.Response.Cookies.Append(cookie.Name, accessToken);
        }

        public static long GetUserIdFromHttpContext(this HttpContext httpContext)
        {
            var useridString = httpContext.User?.FindFirst(TokenClaim.UserId)?.Value!;

            if (string.IsNullOrWhiteSpace(useridString))
                throw new Exception($"Claim {TokenClaim.UserId} is not available");

            return long.Parse(useridString);

        }
        public static void WriteHttpOnlyCookie(this HttpContext context, Cookie cookie, string accessToken)
        {
            context.DeleteCookie(cookie);
            //context.Request.Headers[MyAuthenticationOptions.DefaultScheme] = new StringValues($"Bearer {accessToken}");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            context.Response.Cookies.Append(cookie.Name, accessToken, cookieOptions);
        }

        public static string GetTokenFromContext(this HttpContext context)
        {
            // Check if the token is in the Authorization header
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length);
            }

            // If the token is not found in the Authorization header, check query string
            if (context.Request.Query.TryGetValue("access_token", out var tokenFromQuery))
            {
                return tokenFromQuery;
            }

            // Check if the token is in a custom header (if applicable)
            var customHeader = context.Request.Headers["X-Access-Token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(customHeader))
            {
                return customHeader;
            }

            throw new InvalidOperationException("Token not found in request headers or query string.");
        }


        public static string? ReadCookie(this HttpContext context, Cookie cookie)
        {
            var tokenFromCookie = context.Request.Cookies[cookie.Name];
            if (!string.IsNullOrEmpty(tokenFromCookie))
            {
                return tokenFromCookie;
            }

            if (context.Request.Headers.TryGetValue(MyAuthenticationOptions.DefaultScheme, out StringValues token))
            {
                var tokenParts = token.ToString().Split(" ");
                if (tokenParts.Length == 2 && tokenParts[0] == "Bearer")
                {
                    return tokenParts[1];
                }
            }
            return null;
        }
    }
}
