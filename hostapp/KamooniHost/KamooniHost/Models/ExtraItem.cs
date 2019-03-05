using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class ExtraItem : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string uomId;

        [JsonProperty("uom_id")]
        public string UomId { get => uomId; set => SetProperty(ref uomId, value); }

        private string category;

        [JsonProperty("category")]
        public string Category { get => category; set => SetProperty(ref category, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private double unitPrice;

        [JsonProperty("price")]
        public double UnitPrice { get => unitPrice; set => SetProperty(ref unitPrice, value, onChanged: () => OnPropertyChanged(nameof(Total))); }

        private int quantity;

        [JsonProperty("quantity")]
        public int Quantity { get => quantity; set => SetProperty(ref quantity, value, onChanged: () => OnPropertyChanged(nameof(Total))); }

        public double Total => UnitPrice * Quantity;
    }
}