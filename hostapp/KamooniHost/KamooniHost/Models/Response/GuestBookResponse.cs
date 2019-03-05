using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class GuestBookResponse : BaseResponse
    {
        [JsonProperty("result")]
        public GuestBookResult Result { get; set; }
    }
}