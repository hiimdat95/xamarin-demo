using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class Revenue : BaseModel
    {
        private double total;

        [JsonProperty("total")]
        public double Total { get => total; set => SetProperty(ref total, value, onChanged: () => OnPropertyChanged(nameof(Balance))); }

        private double paid;

        [JsonProperty("paid")]
        public double Paid { get => paid; set => SetProperty(ref paid, value, onChanged: () => OnPropertyChanged(nameof(Balance))); }

        [JsonProperty("balance")]
        public double Balance => Total - paid;
    }
}