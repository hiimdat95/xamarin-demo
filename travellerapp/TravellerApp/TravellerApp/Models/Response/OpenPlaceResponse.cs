using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    class OpenPlaceResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("result")]
        public OpenPlaceResult Result { get; set; }
    }
}
