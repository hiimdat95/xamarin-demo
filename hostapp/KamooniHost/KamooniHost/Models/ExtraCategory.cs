using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class ExtraCategory : BaseModel
    {
        private string name;

        public string Name { get => name; set => SetProperty(ref name, value); }

        private double total;

        [JsonProperty("total")]
        public double Total { get => total; set => SetProperty(ref total, value); }

        private double paid;

        [JsonProperty("paid")]
        public double Paid { get => paid; set => SetProperty(ref paid, value); }

        private double balance;

        [JsonProperty("balance")]
        public double Balance { get => balance; set => SetProperty(ref balance, value); }
    }
}