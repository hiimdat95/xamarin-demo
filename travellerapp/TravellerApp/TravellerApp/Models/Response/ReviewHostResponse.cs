using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
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
