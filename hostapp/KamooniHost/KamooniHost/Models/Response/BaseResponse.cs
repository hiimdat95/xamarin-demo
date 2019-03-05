using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class BaseResponse
    {
        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }
    }
}