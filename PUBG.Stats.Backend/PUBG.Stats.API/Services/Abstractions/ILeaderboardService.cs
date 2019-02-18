using System.Threading.Tasks;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;

namespace PUBG.Stats.API.Services.Abstractions
{
    public interface ILeaderboardService
    {
        Task<Leaderboard> GetLeaderboardAsync(string gameMode);
    }
}
