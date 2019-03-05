using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    class ReviewHostResult
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("review_token")]
        public string ReviewToken { get; set; }
    }
}
