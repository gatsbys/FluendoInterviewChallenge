using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PUBG.Stats.API.Configuration;
using PUBG.Stats.API.Services.Abstractions;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;

namespace PUBG.Stats.API.Services
{
    public class FallbackPlayerService : IFallbackPlayerService
    {
        private readonly InternalComConfiguration _internalComConfiguration;

        public FallbackPlayerService(IOptions<InternalComConfiguration> options)
        {
            _internalComConfiguration = options.Value;
        }

        public async Task<PlayerLifetimeStats> GetPlayerAdHocAsync(string playerName)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage responseMessage =
                await client.GetAsync(string.Format(_internalComConfiguration.GetPlayerAdhocUrlFormat, playerName));

            return JsonConvert.DeserializeObject<PlayerLifetimeStats>(await responseMessage.Content.ReadAsStringAsync());
        }
    }
}
