using KamooniHost.Models.Result;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KamooniHost.Models.Response
{
    class PostsResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("result")]
        public PostsResult Result { get; set; }
    }
}
