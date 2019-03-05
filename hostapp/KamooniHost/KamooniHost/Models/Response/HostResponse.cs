using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class HostResponse : BaseResponse
    {
        [JsonProperty("result")]
        public HostResult Result { get; set; }
    }
}