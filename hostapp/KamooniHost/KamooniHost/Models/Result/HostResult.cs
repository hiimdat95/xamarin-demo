using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class HostResult : BaseResult
    {
        [JsonProperty("host")]
        public Host Host { get; set; }

        [JsonProperty("posts")]
        public List<Post> Posts { get; set; }
    }
}