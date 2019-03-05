using System;
using Newtonsoft.Json;


namespace TravellerApp.Models.Response
{
    public class SuccessCheckoutId
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        [JsonProperty("result")]
        public SuccessResultCheckoutId Result { get; set; }
    }

    public class SuccessResultCheckoutId
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("CheckoutId")]
        public string CheckoutId { get; set; }
    }
}
