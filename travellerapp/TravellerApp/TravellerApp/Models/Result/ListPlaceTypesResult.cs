using Newtonsoft.Json;
using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    class ListPlaceTypesResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("types")]
        public List<Types> Types { get; set; }
    }
}
 