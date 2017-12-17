using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebRanging.Queue
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobType
    {
        ParseSite,
        AnalyzeSite
    }
}