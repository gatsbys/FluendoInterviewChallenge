using System.Threading.Tasks;
using PUBG.Stats.API.Services.Abstractions;
using PUBG.Stats.Core.Services.Data.Abstractions;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;

namespace PUBG.Stats.API.Services
{
    public class PlayerLifetimeStatsService : IPlayerLifetimeStatsService
    {
        private readonly IReadOnlyMongoDbService _readOnlyMongoDbService;
        private readonly string IdFieldKey = "PlayerId";
        private readonly string NameFieldKey = "PlayerName";


        public PlayerLifetimeStatsService(IReadOnlyMongoDbService readOnlyMongoDbService)
        {
            _readOnlyMongoDbService = readOnlyMongoDbService;
        }

        public async Task<PlayerLifetimeStats> GetPlayerStatsById(string id)
        {
            return await _readOnlyMongoDbService.GetDocumentAsync<PlayerLifetimeStats>(IdFieldKey, id);
        }

        public async Task<PlayerLifetimeStats> GetPlayerStatsByName(string name)
        {
            return await _readOnlyMongoDbService.GetDocumentAsync<PlayerLifetimeStats>(NameFieldKey, name);
        }
    }
}
