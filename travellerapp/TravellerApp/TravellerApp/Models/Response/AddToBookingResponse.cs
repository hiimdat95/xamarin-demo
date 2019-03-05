using Newtonsoft.Json;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class AddToBookingResponse : BaseResponse
    {
        [JsonProperty("result")]
        public AddToBookingResult Result { get; set; }
    }
}