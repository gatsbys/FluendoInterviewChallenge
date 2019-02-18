using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;
using PUBG.Stats.Worker.Configuration;
using PUBG.Stats.Worker.Services.Abstractions;

namespace PUBG.Stats.Worker.Services
{
    public class DocumentExtractorService : IDocumentExtractorService
    {
        private readonly string LeaderboardPlayersJsonPath = "included";
        private readonly string DataGameModeStatsJsonPath = "data";
        private readonly string AttributesGameModeStatsJsonPath = "attributes";
        private readonly string GameModeStatsJsonPath = "gameModeStats";

        private readonly WorkerConfiguration _workerConfiguration;

        public DocumentExtractorService(IOptions<WorkerConfiguration> options)
        {
            _workerConfiguration = options.Value;
        }

        public Leaderboard GetLeaderboardDocumentFromRaw(string raw, string gameMode)
        {
            JObject jResponse = JsonConvert.DeserializeObject<JObject>(raw);
            JArray players = JArray.FromObject(jResponse[LeaderboardPlayersJsonPath]);

            Leaderboard document = new Leaderboard();
            foreach (JToken player in players)
            {
                LeaderboardPlayer leaderboardPlayer = player.ToObject<LeaderboardPlayer>();
                document.Players.Add(leaderboardPlayer);
                document.Mode = gameMode;
            }
            return document;
        }


        public PlayerLifetimeStats GetPlayerLifetimeStatsDocumentFromRaw(string raw, string playerId, string name)
        {

            JObject jResponse = JsonConvert.DeserializeObject<JObject>(raw);
            JToken gameModeStats = jResponse[DataGameModeStatsJsonPath][AttributesGameModeStatsJsonPath][GameModeStatsJsonPath];

            PlayerLifetimeStats player = new PlayerLifetimeStats();

            foreach (string gameMode in _workerConfiguration.GameModes)
            {
                player.PlayerId = playerId;
                player.PlayerName = name;

                GameModeStats modeStats = new GameModeStats();
                modeStats.Mode = gameMode;
                modeStats.Stats = gameModeStats[gameMode].ToObject<GameStats>();

                player.GameModeStats.Add(modeStats);
            }

            return player;
        }
    }
}
