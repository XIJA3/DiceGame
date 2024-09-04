using ApplicationTemplate.Server.Common.Interfaces;
using ApplicationTemplate.Server.Common.Security;
using DataTransferModels.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class LeaderboardController(ILeaderboardService leaderboardService) : Controller
    {
        private readonly ILeaderboardService _leaderboardService = leaderboardService;


        [HttpGet("[action]")]
        public async Task<ActionResult<List<UserLeaderboardDto>>> GetTopPlayersByScore()
        {
            var leaderboard = await _leaderboardService.GetTopPlayersByScoreAsync();
            return Ok(leaderboard);
        }
    }
}
