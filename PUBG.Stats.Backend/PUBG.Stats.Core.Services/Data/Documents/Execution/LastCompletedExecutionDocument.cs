using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PUBG.Stats.Core.Services.Data.Documents.Execution
{
    public class LastCompletedExecutionDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public DateTime LastCompletedExecution { get; set; }
    }
}
