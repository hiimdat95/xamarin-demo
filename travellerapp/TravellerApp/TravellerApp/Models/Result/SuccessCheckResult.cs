using Newtonsoft.Json;

namespace TravellerApp.Models.Result
{
    public class SuccessResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}