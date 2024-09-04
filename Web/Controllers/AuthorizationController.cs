using DataTransferModels.Requests;
using DataTransferModels.Responses;
using Microsoft.AspNetCore.Mvc;
using Web.Hubs;
using Web.Services.IServices;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthorizationController(IUserAuthenticationService authenticationService) : Controller
    {
        private readonly IUserAuthenticationService _authenticationService = authenticationService;

        [HttpPost("[action]")]
        public async Task<ActionResult<UserSession>> LogIn(LoginRequest request)
        {
            var userSession = await _authenticationService.LogInAsync(request);

            return userSession;
        }
    }
}
