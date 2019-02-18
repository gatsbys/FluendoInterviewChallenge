using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using PUBG.Stats.Core.Services.Data.Abstractions;
using PUBG.Stats.Core.Services.Data.Documents.Execution;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;
using PUBG.Stats.Worker.Configuration;
using PUBG.Stats.Worker.Services.Abstractions;
using Serilog;

namespace PUBG.Stats.Worker.Services
{
    public class ETLService : IETLService
    {
        private readonly IPUBGApiCaller _pubgApiCaller;
        private readonly IWriteOnlyMongoDbService _writeOnlyMongoDbService;
        private readonly WorkerConfiguration _workerConfiguration;
        private readonly string LastCompletedExecutionKey = "StatsJobExecution";

        public ETLService(
            IPUBGApiCaller pubgApiCaller,
            IWriteOnlyMongoDbService writeOnlyMongoDbService,
            IOptions<WorkerConfiguration> options)
        {
            _pubgApiCaller = pubgApiCaller;
            _writeOnlyMongoDbService = writeOnlyMongoDbService;
            _workerConfiguration = options.Value;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            List<Leaderboard> leaderboards = await RunLeaderBoard(cancellationToken);
            List<PlayerLifetimeStats> playerLifetimeStats = new List<PlayerLifetimeStats>();

            foreach (Leaderboard leaderboard in leaderboards)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                List<PlayerLifetimeStats> currentPlayers = await RunPlayerStatsAsync(leaderboard, cancellationToken);
                playerLifetimeStats.AddRange(currentPlayers);
            }

            await _writeOnlyMongoDbService.WipeCollectionAsync<PlayerLifetimeStats>();
            await _writeOnlyMongoDbService.IndexDocumentsAsync(playerLifetimeStats);

            if (leaderboards.Any())
            {
                await _writeOnlyMongoDbService.ReplaceDocumentAsync(new LastCompletedExecutionDocument()
                { LastCompletedExecution = DateTime.UtcNow, Id = LastCompletedExecutionKey }, c => c.Id == LastCompletedExecutionKey);
            }
        }

        #region Leaderboard

        private async Task<List<Leaderboard>> RunLeaderBoard(CancellationToken cancellationToken)
        {
            List<Leaderboard> leaderboards = new List<Leaderboard>();
            foreach (string gameMode in _workerConfiguration.GameModes)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Log.Information($"Getting leaderboard for game mode {gameMode}");

                    Leaderboard apiResponse = await _pubgApiCaller.GetLeaderBoardAsync(gameMode, cancellationToken);

                    apiResponse.Id = apiResponse.Mode;
                    await _writeOnlyMongoDbService.ReplaceDocumentAsync(apiResponse, c => c.Id == gameMode);

                    leaderboards.Add(apiResponse);

                    Log.Information($"Leaderboard for {gameMode} saved correctly");
                }
                catch (Exception exception)
                {
                    Log.Error($"Error getting mode {gameMode}.", exception);
                }
            }

            return leaderboards;
        }

        #endregion

        #region Player Stats

        private async Task<List<PlayerLifetimeStats>> RunPlayerStatsAsync(Leaderboard leaderboard, CancellationToken cancellationToken)
        {
            List<PlayerLifetimeStats> playerLifetimeStats = new List<PlayerLifetimeStats>();
            foreach (LeaderboardPlayer leaderboardPlayer in leaderboard.Players)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return new List<PlayerLifetimeStats>();
                    }

                    Log.Information($"Getting lifetime stats for player {leaderboardPlayer.Id}");

                    PlayerLifetimeStats response = await _pubgApiCaller.GetPlayerLifetimeStatsAsync(leaderboardPlayer, cancellationToken);
                    response.Id = leaderboardPlayer.Id;

                    playerLifetimeStats.Add(response);

                    Log.Information($"Lifetime stats for player {leaderboardPlayer.Id} saved in memory correctly.");
                }
                catch (Exception)
                {
                    Log.Error($"Error on stats update for player {leaderboardPlayer.Id}");
                }
            }

            return playerLifetimeStats;
        }
        #endregion
    }
}
