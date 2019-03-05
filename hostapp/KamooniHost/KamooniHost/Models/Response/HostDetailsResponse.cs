using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class HostDetailsResponse : BaseResponse
    {
        [JsonProperty("result")]
        public HostDetailsResult Result { get; set; }
    }
}