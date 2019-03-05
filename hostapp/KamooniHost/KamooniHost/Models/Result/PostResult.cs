using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    public class PostResult : BaseModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}