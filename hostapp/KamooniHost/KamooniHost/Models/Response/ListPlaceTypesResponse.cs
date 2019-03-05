using Newtonsoft.Json;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    class ListPlaceTypesResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("result")]
        public ListPlaceTypesResult Result { get; set; }
    }
}
