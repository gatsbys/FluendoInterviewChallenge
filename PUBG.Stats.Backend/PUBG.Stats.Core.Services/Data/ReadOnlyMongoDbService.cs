using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PUBG.Stats.Core.Services.Data.Abstractions;
using PUBG.Stats.Core.Services.Data.Configuration;

namespace PUBG.Stats.Core.Services.Data
{
    public class ReadOnlyMongoDbService : IReadOnlyMongoDbService
    {
        private readonly MongoDbConfiguration _mongoDbConfiguration;

        public ReadOnlyMongoDbService(IOptions<MongoDbConfiguration> options)
        {
            _mongoDbConfiguration = options.Value;
        }

        public async Task<T> GetDocumentAsync<T>(string key, string value) where T : class
        {
            MongoClient client = new MongoClient(_mongoDbConfiguration.ConnectionString);
            IMongoDatabase database = client.GetDatabase(_mongoDbConfiguration.DatabaseName);
            T document = await (await database.GetCollection<T>(CollectionDiscover.GetCollection<T>()).FindAsync(new BsonDocument(key, value))).FirstOrDefaultAsync();
            return document;
        }

        public async Task<T> GetSingleDocumentAsync<T>() where T : class
        {
            MongoClient client = new MongoClient(_mongoDbConfiguration.ConnectionString);
            IMongoDatabase database = client.GetDatabase(_mongoDbConfiguration.DatabaseName);
            T document = await (await database.GetCollection<T>(CollectionDiscover.GetCollection<T>()).FindAsync(new BsonDocument())).FirstOrDefaultAsync();
            return document;
        }
    }
}
