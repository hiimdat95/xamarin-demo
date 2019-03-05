using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    public class HostTokenResult : BaseResult
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}