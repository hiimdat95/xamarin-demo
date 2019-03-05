using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response
{
    public class SendPaymentLinkResponse : BaseResponse
    {
        [JsonProperty("result")]
        public SendPaymentLinkResult Result { get; set; }
    }
}