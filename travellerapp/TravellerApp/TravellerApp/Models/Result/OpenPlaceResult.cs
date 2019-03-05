using Newtonsoft.Json;
using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    class OpenPlaceResult
    {
        [JsonProperty("reviews")]
        public List<review> Reviews { get; set; }

        [JsonProperty("founder")]
        public Founder Founder { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
