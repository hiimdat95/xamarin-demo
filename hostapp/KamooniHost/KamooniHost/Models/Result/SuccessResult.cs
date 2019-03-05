using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    class SuccessResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
