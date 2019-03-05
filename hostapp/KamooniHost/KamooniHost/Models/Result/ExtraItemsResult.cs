using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class ExtraItemsResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("products")]
        public List<ExtraItem> ListExtra { get; set; }
    }
}