using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class BillItem : BaseModel
    {
        private string category;

        [JsonProperty("category")]
        public string Category { get => category; set => SetProperty(ref category, value); }

        private string product;

        [JsonProperty("product")]
        public string Product { get => product; set => SetProperty(ref product, value); }

        private int quantity;

        [JsonProperty("quantity")]
        public int Quantity { get => quantity; set => SetProperty(ref quantity, value); }

        private double netPrice;

        [JsonProperty("total")]
        public double NetPrice { get => netPrice; set => SetProperty(ref netPrice, value); }

        private double grossPrice;

        [JsonProperty("amount")]
        public double GrossPrice { get => grossPrice; set => SetProperty(ref grossPrice, value); }

        private double discount;

        [JsonProperty("discount")]
        public double Discount { get => discount; set => SetProperty(ref discount, value); }
    }
}