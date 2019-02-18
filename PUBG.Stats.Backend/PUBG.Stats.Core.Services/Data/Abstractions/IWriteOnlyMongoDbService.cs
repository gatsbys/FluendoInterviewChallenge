using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PUBG.Stats.Core.Services.Data.Abstractions
{
    public interface IWriteOnlyMongoDbService
    {
        Task IndexDocumentAsync<T>(T document) where T : class;

        Task ReplaceDocumentAsync<T>(T document, Expression<Func<T, bool>> match) where T : class;

        Task WipeCollectionAsync<T>() where T : class;

        Task IndexDocumentsAsync<T>(List<T> documents) where T : class;
    }
}
