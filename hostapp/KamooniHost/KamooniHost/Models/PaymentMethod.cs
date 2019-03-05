using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class PaymentMethod : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }
    }
}