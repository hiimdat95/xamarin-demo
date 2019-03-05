using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class ExtraItemsResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("result")]
        public ExtraItemsResult Result { get; set; }
    }
}