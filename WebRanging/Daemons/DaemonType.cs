using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebRanging.Daemons
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DaemonType
    {
        Parser,
        Analyzer
    }
}