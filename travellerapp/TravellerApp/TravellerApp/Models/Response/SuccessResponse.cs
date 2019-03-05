using Newtonsoft.Json;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class SuccessResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        [JsonProperty("result")]
        public SuccessResult Result { get; set; }
    }
}