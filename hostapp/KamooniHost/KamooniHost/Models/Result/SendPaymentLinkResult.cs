using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    public class SendPaymentLinkResult : BaseResult
    {
        [JsonProperty("payment_link")]
        public string PaymentLink { get; set; }
    }
}