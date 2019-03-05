using Newtonsoft.Json;     

namespace TravellerApp.Models.Result
{
    public class PaymentResult : BaseResult
    {
        [JsonProperty("payment_link")]
        public string PaymentLink { get; set; }
    }
}
