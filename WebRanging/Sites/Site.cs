using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebRanging.Sites
{
    public class Site
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Url { get; set; }
        public Guid BundleVersion { get; set; }
    }
}