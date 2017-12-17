using MongoDB.Bson;
using Newtonsoft.Json;

namespace WebRanging.Queue
{
    public class QueueItem
    {
        [JsonIgnore]
        public ObjectId _id { get; set; }
    }
}