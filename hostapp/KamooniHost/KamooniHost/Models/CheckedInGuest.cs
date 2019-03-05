using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class CheckedInGuest : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private double amount;

        [JsonProperty("amount")]
        public double Amount { get => amount; set => SetProperty(ref amount, value); }

        private double paid;

        [JsonProperty("paid")]
        public double Paid { get => paid; set => SetProperty(ref paid, value); }

        private double balance;

        [JsonProperty("balance")]
        public double Balance { get => balance; set => SetProperty(ref balance, value, onChanged: () => OnPropertyChanged(nameof(CanPay))); }

        public bool CanPay => Balance > 0;
    }
}