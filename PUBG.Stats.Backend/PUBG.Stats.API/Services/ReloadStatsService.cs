using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PUBG.Stats.API.Configuration;
using PUBG.Stats.API.Services.Abstractions;
using PUBG.Stats.Core.Services.Data.Abstractions;
using PUBG.Stats.Core.Services.Data.Documents.Execution;

namespace PUBG.Stats.API.Services
{
    public class ReloadStatsService : IReloadStatsService
    {
        private readonly IReadOnlyMongoDbService _readOnlyMongoDbService;
        private readonly InternalComConfiguration _internalComConfiguration;

        public ReloadStatsService(IReadOnlyMongoDbService readOnlyMongoDbService,
            IOptions<InternalComConfiguration> options)
        {
            _readOnlyMongoDbService = readOnlyMongoDbService;
            _internalComConfiguration = options.Value;
        }

        public async Task ReloadIfNeededAsync()
        {
            LastCompletedExecutionDocument lastCompletedExecution = await _readOnlyMongoDbService.GetSingleDocumentAsync<LastCompletedExecutionDocument>();
            if (lastCompletedExecution == null || (DateTime.UtcNow - lastCompletedExecution.LastCompletedExecution).Hours >= _internalComConfiguration.HoursToForce)
            {
                HttpClient client = new HttpClient();
                Console.WriteLine(_internalComConfiguration.ForceSyncUrl);
                await client.PostAsync(_internalComConfiguration.ForceSyncUrl, new StringContent(string.Empty));
            }
        }
    }
}
