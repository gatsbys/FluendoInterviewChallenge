using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;
using PUBG.Stats.Worker.Configuration;
using PUBG.Stats.Worker.RateManager;
using PUBG.Stats.Worker.Services.Abstractions;
using Serilog;

namespace PUBG.Stats.Worker.Services
{
    public class PUBGApiCaller : IPUBGApiCaller
    {
        private readonly string DataJsonPath = "data";
        private readonly string IdJsonPath = "id";

        private readonly IDocumentExtractorService _documentExtractorService;
        private readonly HttpClient _httpClient;

        public PUBGApiCaller(
            IDocumentExtractorService documentExtractorService,
            HttpClient httpClient)
        {
            _documentExtractorService = documentExtractorService;
            _httpClient = httpClient;
        }


        public async Task<Leaderboard> GetLeaderBoardAsync(string gameMode, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage apiResponse = await CallGovernor.Call(async () =>
                    {
                        return await _httpClient.GetAsync(
                            $"https://api.pubg.com/shards/steam/leaderboards/{gameMode}?page[number]=1", cancellationToken);

                    }, cancellationToken);

                string raw = await apiResponse.Content.ReadAsStringAsync();

                Leaderboard leaderboard =
                    _documentExtractorService.GetLeaderboardDocumentFromRaw(raw, gameMode);

                return leaderboard;
            }
            catch (Exception ex)
            {
                Log.Error("Error getting data from API", ex);
                throw;
            }
        }

        public async Task<PlayerLifetimeStats> GetPlayerLifetimeStatsAsync(LeaderboardPlayer player, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage apiResponse = await CallGovernor.Call(async () =>
            {
                return await _httpClient.GetAsync($"https://api.pubg.com/shards/steam/players/{player.Id}/seasons/lifetime", cancellationToken);

            }, cancellationToken);

                string raw = await apiResponse.Content.ReadAsStringAsync();

                PlayerLifetimeStats playerLifetimeStats =
                    _documentExtractorService.GetPlayerLifetimeStatsDocumentFromRaw(raw, player.Id, player.Attributes.Name);

                return playerLifetimeStats;
            }
            catch (Exception ex)
            {
                Log.Error("Error getting data from API", ex);
                throw;
            }
        }

        public async Task<PlayerLifetimeStats> GetPlayerLifetimeStatsByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                string playerId = await GetPlayerIdAsync(name, cancellationToken);

                HttpResponseMessage apiResponse = await CallGovernor.Call(async () =>
                {
                    return await _httpClient.GetAsync(
                        $"https://api.pubg.com/shards/steam/players/{playerId}/seasons/lifetime", cancellationToken);
                }, cancellationToken);

                string raw = await apiResponse.Content.ReadAsStringAsync();

                PlayerLifetimeStats playerLifetimeStats =
                    _documentExtractorService.GetPlayerLifetimeStatsDocumentFromRaw(raw, playerId, name);

                return playerLifetimeStats;
            }
            catch (Exception ex)
            {
                Log.Error("Error getting data from API", ex);
                throw;
            }
        }

        #region Private methods

        private async Task<string> GetPlayerIdAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage apiResponse = await CallGovernor.Call(async () =>
            {
                return await _httpClient.GetAsync($"https://api.pubg.com/shards/steam/players?filter[playerNames]={name}", cancellationToken);
            }, cancellationToken);

                string raw = await apiResponse.Content.ReadAsStringAsync();

                JObject jResponse = JsonConvert.DeserializeObject<JObject>(raw);
                JToken data = jResponse[DataJsonPath].ToObject<JArray>().First();
                string id = data[IdJsonPath].Value<string>();
                return id;
            }
            catch (Exception ex)
            {
                Log.Error("Error getting data from API", ex);
                throw;
            }
        }

        #endregion
    }
}
