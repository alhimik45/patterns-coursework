using System;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace WebRanging.Sites
{
    public class Site
    {
        [JsonIgnore]
        public ObjectId _id { get; set; }
        public Guid BundleVersion { get; set; }
    }
}