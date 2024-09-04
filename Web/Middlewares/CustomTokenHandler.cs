using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Web.Services;
using static Web.Helpers.MiddlewareExtensions;

namespace Web.Middlewares
{
    public class CustomTokenHandler(
        CustomAuthentication customAuthentication,
        IOptionsMonitor<MyAuthenticationOptions> options,
        ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock)
            : AuthenticationHandler<MyAuthenticationOptions>(options, logger, encoder, clock)
    {
        private readonly CustomAuthentication _customAuthentication = customAuthentication;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var result = await _customAuthentication.AuthenticateAsync(Context);

                if (!result.Succeeded)
                    return AuthenticateResult.Fail("Can Not Authenticate User");

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = GetErrorMessageForStatusCode((HttpStatusCode)Context.Response.StatusCode, ex);
                await Context.Response.WriteAsync(errorMessage);

                // Todo: Log
                throw;
            }
        }
    }
}
