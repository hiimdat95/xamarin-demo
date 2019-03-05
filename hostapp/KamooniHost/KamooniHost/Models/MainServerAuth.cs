using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class MainServerAuth
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("id")]
        public int HostId { get; set; }

        [JsonProperty("token")]
        public string HostToken { get; set; }
    }
}