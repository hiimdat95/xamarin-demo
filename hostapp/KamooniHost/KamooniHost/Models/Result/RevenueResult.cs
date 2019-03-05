using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    public class RevenueResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("revenue")]
        public Revenue Revenue { get; set; }
    }
}