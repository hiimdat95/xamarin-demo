using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    class SuccessResponse
    {
        public string jsonrpc { get; set; }
        public int? id { get; set; }

        [JsonProperty("result")]
        public SuccessResult Result { get; set; }
    }
}
