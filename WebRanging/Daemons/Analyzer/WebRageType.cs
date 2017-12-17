using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebRanging.Daemons.Analyzer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WebRageType
    {
        Page,
        BigText,
        Links
    }
}