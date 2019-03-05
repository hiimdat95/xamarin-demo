using Newtonsoft.Json;
using System;

namespace KamooniHost.Models.Profile
{
    public class Purchase : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string item;

        [JsonProperty("item")]
        public string Item { get => item; set => SetProperty(ref item, value); }

        public double price;

        [JsonProperty("price")]
        public double Price { get => price; set => SetProperty(ref price, value); }

        private string currency;

        [JsonProperty("currency")]
        public string Currency { get => currency; set => SetProperty(ref currency, value); }

        private string currency_human;

        [JsonProperty("currency_human")]
        public string CurrencyHuman { get => currency_human; set => SetProperty(ref currency_human, value); }

        private string sku;

        [JsonProperty("sku")]
        public string Sku { get => sku; set => SetProperty(ref sku, value); }

        private Representative representative = new Representative();

        [JsonProperty("representative")]
        public Representative Representative { get => representative; set => SetProperty(ref representative, value); }

        private Venue venue = new Venue();

        [JsonProperty("venue")]
        public Venue Venue { get => venue; set => SetProperty(ref venue, value); }

        private DateTimeOffset date_added;

        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get => date_added; set => SetProperty(ref date_added, value); }

        private DateTimeOffset date_changed;

        [JsonProperty("date_changed")]
        public DateTimeOffset DateChanged { get => date_changed; set => SetProperty(ref date_changed, value); }

        private Guid external_id;

        [JsonProperty("external_id")]
        public Guid ExternalId { get => external_id; set => SetProperty(ref external_id, value); }
    }
}