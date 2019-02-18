using System.Threading;
using System.Threading.Tasks;

namespace PUBG.Stats.Worker.Services.Abstractions
{
    public interface IETLService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
