using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PUBG.Stats.Core.Hosting;
using PUBG.Stats.Worker.Services.Abstractions;

namespace PUBG.Stats.Worker.InternalComApi
{
    [Route("api/worker/[controller]")]
    [ApiController]
    public class AdHocController : ControllerBase
    {
        private readonly IPUBGApiCaller _pubgApiCaller;
        private readonly IJob _job;

        public AdHocController(IPUBGApiCaller pubgApiCaller, IJob job)
        {
            _pubgApiCaller = pubgApiCaller;
            _job = job;
        }

        [HttpGet("by-name/{playerName}")]
        public async Task<ActionResult> GetAdHocLifetimeStatsByName(string playerName)
        {
            return Ok(await _pubgApiCaller.GetPlayerLifetimeStatsByNameAsync(playerName, new CancellationToken()));
        }

        [HttpPost("startsync")]
        public ActionResult StartSyncAsync(string playerName)
        {
            _job.ForceStart();
            return Accepted();
        }
    }
}