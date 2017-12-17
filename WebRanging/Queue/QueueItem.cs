using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebRanging.Queue
{
    public class QueueItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public JobType JobType { get; set; }
        public Dictionary<string,string> Arguments { get; set; } = new Dictionary<string, string>();
        public DateTime AdditionTime { get; set; } = DateTime.Now;
    }
}