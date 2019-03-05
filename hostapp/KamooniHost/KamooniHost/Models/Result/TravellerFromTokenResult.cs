using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    public class TravellerFromTokenResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("traveller")]
        public TravellerFromQR TravellerFromQR { get; set; }
    }
}