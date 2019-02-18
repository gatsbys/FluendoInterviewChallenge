using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PUBG.Stats.Core.Services.Data.Abstractions;
using PUBG.Stats.Core.Services.Data.Configuration;

namespace PUBG.Stats.Core.Services.Data
{
    public class WriteOnlyMongoDbService : IWriteOnlyMongoDbService
    {
        private readonly MongoDbConfiguration _mongoDbConfiguration;

        public WriteOnlyMongoDbService(IOptions<MongoDbConfiguration> options)
        {
            _mongoDbConfiguration = options.Value;
        }

        public async Task IndexDocumentAsync<T>(T document) where T : class
        {
            MongoClient client = new MongoClient(_mongoDbConfiguration.ConnectionString);
            IMongoDatabase database = client.GetDatabase(_mongoDbConfiguration.DatabaseName);

            await database.GetCollection<T>(CollectionDiscover.GetCollection<T>()).InsertOneAsync( document);
        }

        public async Task ReplaceDocumentAsync<T>(T document, Expression<Func<T, bool>> match) where T : class
        {
            MongoClient client = new MongoClient(_mongoDbConfiguration.ConnectionString);
            IMongoDatabase database = client.GetDatabase(_mongoDbConfiguration.DatabaseName);

            ReplaceOneResult replaceOneResult = await database.GetCollection<T>(CollectionDiscover.GetCollection<T>()).ReplaceOneAsync(match, document);
            if (replaceOneResult.MatchedCount == 0)
            {
                await IndexDocumentAsync(document);
            }
        }

        public async Task IndexDocumentsAsync<T>(List<T> documents) where T : class
        {
            MongoClient client = new MongoClient(_mongoDbConfiguration.ConnectionString);
            IMongoDatabase database = client.GetDatabase(_mongoDbConfiguration.DatabaseName);

            await database.GetCollection<T>(CollectionDiscover.GetCollection<T>()).InsertManyAsync(documents);
        }

        public async Task WipeCollectionAsync<T>() where T : class
        {
            MongoClient client = new MongoClient(_mongoDbConfiguration.ConnectionString);
            IMongoDatabase database = client.GetDatabase(_mongoDbConfiguration.DatabaseName);
            await database.GetCollection<T>(CollectionDiscover.GetCollection<T>()).DeleteManyAsync(arg => true);
        }
    }
}
