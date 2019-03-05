using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Response
{
    public class DatabasesResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("result")]
        public List<string> Result { get; set; }
    }
}