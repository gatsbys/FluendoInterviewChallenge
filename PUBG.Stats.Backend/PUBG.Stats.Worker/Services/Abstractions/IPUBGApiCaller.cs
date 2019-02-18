using System.Threading;
using System.Threading.Tasks;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;

namespace PUBG.Stats.Worker.Services.Abstractions
{
    public interface IPUBGApiCaller
    {
        Task<Leaderboard> GetLeaderBoardAsync(string gameMode, CancellationToken cancellationToken);

        Task<PlayerLifetimeStats> GetPlayerLifetimeStatsAsync(LeaderboardPlayer player,
            CancellationToken cancellationToken);

        Task<PlayerLifetimeStats> GetPlayerLifetimeStatsByNameAsync(string name,
            CancellationToken cancellationToken);
    }
}
