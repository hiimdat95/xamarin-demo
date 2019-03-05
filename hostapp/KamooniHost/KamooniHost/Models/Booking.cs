using Newtonsoft.Json;
using System;
using Xamarin.Forms.Converters;

namespace KamooniHost.Models
{
    public class Booking : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string reference;

        [JsonProperty("reference")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public string Reference { get => reference; set => SetProperty(ref reference, value); }

        private DateTime? arrive;

        [JsonProperty("arrive")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public DateTime? Arrive { get => arrive; set => SetProperty(ref arrive, value); }

        private DateTime? depart;

        [JsonProperty("depart")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public DateTime? Depart { get => depart; set => SetProperty(ref depart, value); }

        private string dates;

        [JsonProperty("dates")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public string Dates { get => dates; set => SetProperty(ref dates, value); }

        private string state;

        [JsonProperty("state")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public string State { get => state; set => SetProperty(ref state, value); }

        private double totalPrice;

        [JsonProperty("total")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public double TotalPrice { get => totalPrice; set => SetProperty(ref totalPrice, value); }

        private double paidPrice;

        [JsonProperty("paid")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public double PaidPrice { get => paidPrice; set => SetProperty(ref paidPrice, value, onChanged: () => OnPropertyChanged(nameof(CanPayNow))); }

        private double balancePrice;

        [JsonProperty("balance")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public double BalancePrice { get => balancePrice; set => SetProperty(ref balancePrice, value, onChanged: () => OnPropertyChanged(nameof(CanCheckOut))); }

        private bool isPaid;

        [JsonProperty("is_paid")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public bool IsPaid { get => isPaid; set => SetProperty(ref isPaid, value, onChanged: () => OnPropertyChanged(nameof(ButtonText), nameof(ButtonColor))); }

        public string ButtonText => IsPaid ? "Paid" : "Pay";

        public string ButtonColor => IsPaid ? "Green" : "Orange";

        public bool CanPayNow => BalancePrice > 0;

        public bool CanCheckOut => BalancePrice <= 0;
    }
}