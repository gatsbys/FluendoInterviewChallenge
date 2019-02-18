using System.Threading.Tasks;

namespace PUBG.Stats.API.Services.Abstractions
{
    public interface IReloadStatsService
    {
        Task ReloadIfNeededAsync();
    }
}
