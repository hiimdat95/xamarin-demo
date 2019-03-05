using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class GuestProfileResponse : BaseResponse
    {
        [JsonProperty("result")]
        public GuestProfileResult Result { get; set; }
    }
}