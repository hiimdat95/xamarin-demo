using Newtonsoft.Json;
using System.Collections.Generic;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    class ListPlacesResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("result")]
        public ListPlacesResult Result { get; set; }
    }
}
