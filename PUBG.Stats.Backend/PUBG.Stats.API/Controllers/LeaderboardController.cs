using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PUBG.Stats.API.Configuration;
using PUBG.Stats.API.Controllers.Base;
using PUBG.Stats.API.Filters;
using PUBG.Stats.API.Services.Abstractions;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;

namespace PUBG.Stats.API.Controllers
{
    /// <summary>
    /// Endpoint to get leaderboard stats.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class LeaderboardController : FluendoController
    {
        private readonly ILeaderboardService _leaderboardService;

        /// <inheritdoc cref="LeaderboardController"/>
        public LeaderboardController(
            ILeaderboardService leaderboardService, 
            IDistributedCache distributedCache,
            IOptions<CacheConfiguration> options) : base(distributedCache, "leaderboard-stats:{0}", options)
        {
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Returns the leaderboard info for the given game mode.
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
        [HttpGet("{gameMode}")]
        [ProducesResponseType(typeof(Leaderboard), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [RequiredToken]
        public async Task<ActionResult<Leaderboard>> Get(string gameMode)
        {
            Leaderboard leaderboard = await CacheAsideAsync<Leaderboard>(gameMode,
                async (mode) => await _leaderboardService.GetLeaderboardAsync(mode));

            if (leaderboard == null)
            {
                return NotFound(gameMode);
            }

            return Ok(leaderboard);
        }

        
    }
}