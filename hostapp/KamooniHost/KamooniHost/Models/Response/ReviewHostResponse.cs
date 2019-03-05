using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    class ReviewHostResponse
    {
        [JsonProperty("id")]
        public string jsonrpc { get; set; }
        [JsonProperty("jsonrpc")]
        public int? id { get; set; }
        [JsonProperty("result")]
        public ReviewHostResult Result { get; set; }
    }
}
