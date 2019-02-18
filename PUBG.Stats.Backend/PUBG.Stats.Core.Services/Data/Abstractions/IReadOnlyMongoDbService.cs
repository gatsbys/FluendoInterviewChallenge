using System.Threading.Tasks;

namespace PUBG.Stats.Core.Services.Data.Abstractions
{
    public interface IReadOnlyMongoDbService
    {
        Task<T> GetDocumentAsync<T>(string key, string value) where T : class;

        Task<T> GetSingleDocumentAsync<T>() where T : class;
    }
}
