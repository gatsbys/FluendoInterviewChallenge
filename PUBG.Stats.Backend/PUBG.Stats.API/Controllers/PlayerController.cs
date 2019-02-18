using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PUBG.Stats.API.Configuration;
using PUBG.Stats.API.Controllers.Base;
using PUBG.Stats.API.Filters;
using PUBG.Stats.API.Services.Abstractions;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;

namespace PUBG.Stats.API.Controllers
{
    /// <summary>
    /// Endpoint to check player stats.
    /// </summary>
    [Route("[controller]/stats")]
    [ApiController]
    public class PlayerController : FluendoController
    {
        private readonly IPlayerLifetimeStatsService _playerLifetimeStatsService;
        private readonly IReloadStatsService _reloadStatsService;
        private readonly IFallbackPlayerService _fallbackPlayerService;

        /// <inheritdoc cref="PlayerController"/>
        public PlayerController(
            IPlayerLifetimeStatsService playerLifetimeStatsService,
            IDistributedCache distributedCache,
            IOptions<CacheConfiguration> options, 
            IReloadStatsService reloadStatsService, 
            IFallbackPlayerService fallbackPlayerService) : base(distributedCache, "lifetime-player-stats:{0}", options)
        {
            _playerLifetimeStatsService = playerLifetimeStatsService;
            _reloadStatsService = reloadStatsService;
            _fallbackPlayerService = fallbackPlayerService;
        }

        /// <summary>
        /// Returns the stats for the given player id. 
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{accountId}")]
        [ProducesResponseType(typeof(PlayerLifetimeStats),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [RequiredToken]
        public async Task<ActionResult<PlayerLifetimeStats>> GetAccountIdAsync(string accountId)
        {
            await _reloadStatsService.ReloadIfNeededAsync();
            PlayerLifetimeStats playerLifetimeStats = await CacheAsideAsync<PlayerLifetimeStats>(accountId,
                async (mode) => await _playerLifetimeStatsService.GetPlayerStatsById(mode));

            if (playerLifetimeStats == null)
            {
                return NotFound(accountId);
            }

            return Ok(playerLifetimeStats);
        }

        /// <summary>
        /// Returns the stats for the given player name, if the account is not on Fluendo system it will be retrieved from PUBG Api via backend system. 
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        [HttpGet("by-name/{playerName}")]
        [ProducesResponseType(typeof(PlayerLifetimeStats), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [RequiredToken]
        public async Task<ActionResult<PlayerLifetimeStats>> GetAccountNameAsync(string playerName)
        {
            await _reloadStatsService.ReloadIfNeededAsync();

            PlayerLifetimeStats playerLifetimeStats = await CacheAsideAsync<PlayerLifetimeStats>(playerName,
                async (mode) => await _playerLifetimeStatsService.GetPlayerStatsByName(mode));

            if (playerLifetimeStats == null)
            {
                playerLifetimeStats = await _fallbackPlayerService.GetPlayerAdHocAsync(playerName);
                if (playerLifetimeStats != null)
                {
                    await Cache(playerName, playerLifetimeStats);
                    return Ok(playerLifetimeStats);
                }
            }
            else
            {
                return Ok(playerLifetimeStats);
            }

            return NotFound(playerName);
        }
    }
}