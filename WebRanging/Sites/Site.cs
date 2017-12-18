using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WebRanging.Daemons.Analyzer;

namespace WebRanging.Sites
{
    public class Site
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Id { get; set; }

        public string Url { get; set; }
        public Guid BundleVersion { get; set; }
        public Dictionary<WebRageType, long> Params { get; set; } = new Dictionary<WebRageType, long>();
    }
}