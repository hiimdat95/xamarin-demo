using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class PastPayment : BaseModel
    {
        private string guestName;

        [JsonProperty("name")]
        public string GuestName { get => guestName; set => SetProperty(ref guestName, value); }

        private string method;

        [JsonProperty("method")]
        public string Method { get => method; set => SetProperty(ref method, value); }

        private double amount;

        [JsonProperty("amount")]
        public double Amount { get => amount; set => SetProperty(ref amount, value); }
    }
}