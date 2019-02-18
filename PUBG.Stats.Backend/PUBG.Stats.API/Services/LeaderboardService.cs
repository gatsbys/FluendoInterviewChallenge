using System.Threading.Tasks;
using PUBG.Stats.API.Services.Abstractions;
using PUBG.Stats.Core.Services.Data.Abstractions;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;

namespace PUBG.Stats.API.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IReadOnlyMongoDbService _readOnlyMongoDbService;
        private readonly string ModeFieldKey = "Mode";

        public LeaderboardService(IReadOnlyMongoDbService readOnlyMongoDbService)
        {
            _readOnlyMongoDbService = readOnlyMongoDbService;
        }

        public async Task<Leaderboard> GetLeaderboardAsync(string gameMode)
        {
            return await _readOnlyMongoDbService.GetDocumentAsync<Leaderboard>(ModeFieldKey, gameMode);
        }
    }
}
