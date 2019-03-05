using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class ExtraBought : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string product;

        [JsonProperty("product")]
        public string Product { get => product; set => SetProperty(ref product, value); }

        private string guestName;

        [JsonProperty("guest")]
        public string GuestName { get => guestName; set => SetProperty(ref guestName, value); }

        private int quantity;

        [JsonProperty("nights")]
        public int Quantity { get => quantity; set => SetProperty(ref quantity, value); }

        private double grossPrice;

        [JsonProperty("price")]
        public double GrossPrice { get => grossPrice; set => SetProperty(ref grossPrice, value); }

        private double discount;

        [JsonProperty("discount")]
        public double Discount { get => discount; set => SetProperty(ref discount, value); }

        private double netPrice;

        [JsonProperty("total")]
        public double NetPrice { get => netPrice; set => SetProperty(ref netPrice, value); }
    }
}