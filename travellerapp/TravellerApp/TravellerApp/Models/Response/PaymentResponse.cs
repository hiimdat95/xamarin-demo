using Newtonsoft.Json;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class PaymentResponse : BaseResponse
    {
        [JsonProperty("result")]
        public PaymentResult Result { get; set; }
    }
}