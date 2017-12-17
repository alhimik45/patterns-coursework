using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebRanging.Queue
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobType
    {
        PARSE_SITE,
        ANALYZE_SITE
    }
}