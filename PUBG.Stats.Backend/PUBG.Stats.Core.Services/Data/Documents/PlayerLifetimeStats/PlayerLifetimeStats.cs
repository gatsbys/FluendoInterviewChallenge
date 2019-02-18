using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PUBG.Stats.Core.Services.Data.Documents.PlayerLifetimeStats
{
    public class PlayerLifetimeStats
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public string PlayerId { get; set; }

        public string PlayerName { get; set; }

        public List<GameModeStats> GameModeStats { get; set; } = new List<GameModeStats>();
    }
}
