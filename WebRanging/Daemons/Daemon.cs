using MongoDB.Bson;
using Newtonsoft.Json;

namespace WebRanging.Daemons
{
    public class Daemon
    {
        [JsonIgnore]
        public ObjectId _id { get; set; }
    }
}