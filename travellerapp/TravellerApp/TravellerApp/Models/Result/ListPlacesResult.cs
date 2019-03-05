using Newtonsoft.Json;
using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    class ListPlacesResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("places")]
        public List<Places> places { get; set; }
    }
}
 