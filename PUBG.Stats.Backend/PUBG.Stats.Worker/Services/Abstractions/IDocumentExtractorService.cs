using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;

namespace PUBG.Stats.Worker.Services.Abstractions
{
    public interface IDocumentExtractorService
    {
        Leaderboard GetLeaderboardDocumentFromRaw(string raw, string gameMode);

        PlayerLifetimeStats GetPlayerLifetimeStatsDocumentFromRaw(string raw, string playerId, string name);
    }
}
