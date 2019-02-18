using System;
using System.Collections.Generic;
using PUBG.Stats.Core.Services.Data.Documents.Execution;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;
using PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats;

namespace PUBG.Stats.Core.Services.Data
{
    public static class CollectionDiscover
    {
        private static readonly IDictionary<Type, string> CollectionStrategies = new Dictionary<Type, string>()
        {
            { typeof(Leaderboard),"leaderboard" },
            { typeof(PlayerLifetimeStats),"lifetime-stats" },
            { typeof(LastCompletedExecutionDocument),"last-completed-execution" }
        };

        public static string GetCollection<T>()
        {
            if (CollectionStrategies.TryGetValue(typeof(T), out string collection))
            {
                return collection;
            }

            throw new ArgumentOutOfRangeException("Cannot match the Document with a collection.");
        }
    }
}
