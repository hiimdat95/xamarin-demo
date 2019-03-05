using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.RestClient
{
    public class Param
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }

    public class FunctionParams
    {
        [JsonProperty("params")]
        public List<Param> Params;
    }
}