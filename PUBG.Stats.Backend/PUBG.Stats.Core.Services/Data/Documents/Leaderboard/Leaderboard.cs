using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PUBG.Stats.Core.Services.Data.Documents.Leaderboard
{
    public class Leaderboard
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public List<LeaderboardPlayer> Players { get; set; } = new List<LeaderboardPlayer>();

        public string Mode { get; set; }
    }
}
