using KamooniHost.Models.Response;
using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    public class HostDetailsResult : BaseResult
    {
        [JsonProperty("host_name")]
        public string HostName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("auth")]
        public MainServerAuth MainServerAuth { get; set; }

        [JsonProperty("terms")]
        public string Terms { get; set; }

        [JsonProperty("url_terms")]
        public string UrlTerms { get; set; }
    }
}