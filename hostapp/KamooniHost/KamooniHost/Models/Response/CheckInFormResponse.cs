using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class CheckInFormResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("result")]
        public CheckInFormResult Result { get; set; }
    }
}