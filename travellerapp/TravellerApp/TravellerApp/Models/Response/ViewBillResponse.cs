using Newtonsoft.Json;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class ViewBillResponse : BaseResponse
    {
        [JsonProperty("result")]
        public ViewBillResult Result { get; set; }
    }
}