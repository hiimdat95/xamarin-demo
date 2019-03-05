using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class GuestScanResponse : BaseResponse
    {
        [JsonProperty("result")]
        public GuestScanResult Result { get; set; }
    }
}