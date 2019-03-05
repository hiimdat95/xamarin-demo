using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class ErrorResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("error")]
        public ErrorResult Result { get; set; }
    }
}