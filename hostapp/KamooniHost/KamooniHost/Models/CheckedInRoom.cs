using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class CheckedInRoom : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("product")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private int nights;

        [JsonProperty("nights")]
        public int Nights { get => nights; set => SetProperty(ref nights, value); }

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