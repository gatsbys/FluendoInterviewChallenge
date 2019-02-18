using System.Threading.Tasks;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;

namespace PUBG.Stats.API.Services.Abstractions
{
    public interface IPlayerLifetimeStatsService
    {
        Task<PlayerLifetimeStats> GetPlayerStatsById(string id);

        Task<PlayerLifetimeStats> GetPlayerStatsByName(string name);
    }
}
