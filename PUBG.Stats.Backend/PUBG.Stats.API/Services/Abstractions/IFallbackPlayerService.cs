using System.Threading.Tasks;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;

namespace PUBG.Stats.API.Services.Abstractions
{
    public interface IFallbackPlayerService
    {
        Task<PlayerLifetimeStats> GetPlayerAdHocAsync(string playerName);
    }
}
