using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class CheckOut : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private double totalPrice;

        [JsonProperty("total")]
        public double TotalPrice { get => totalPrice; set => SetProperty(ref totalPrice, value); }

        private double paidPrice;

        [JsonProperty("paid")]
        public double PaidPrice { get => paidPrice; set => SetProperty(ref paidPrice, value, onChanged: () => OnPropertyChanged(nameof(CanPayNow))); }

        private double balancePrice;

        [JsonProperty("balance")]
        public double BalancePrice { get => balancePrice; set => SetProperty(ref balancePrice, value, onChanged: () => OnPropertyChanged(nameof(CanCheckOut))); }

        public bool CanPayNow => BalancePrice > 0;

        public bool CanCheckOut => BalancePrice <= 0;
    }
}