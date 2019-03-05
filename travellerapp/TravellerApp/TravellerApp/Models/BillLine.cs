using Newtonsoft.Json;
using System;

namespace TravellerApp.Models
{
    public class BillLine : BaseModel
    {
        [JsonProperty("categ_id")]
        public string CategId { get; set; }

        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("unit")]
        public long Unit { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("discount_amount")]
        public long DiscountAmount { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        public string DateCreate
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Date.ToString()) && !"false".Equals(Date.ToString()))
                {
                    return Date.ToString("yyyy-MM-dd");
                }

                return default;
            }
        }

        public string DateTimeCreate
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Date.ToString()) && !"false".Equals(Date.ToString()))
                {
                    return Date.ToString("yyyy-MM-dd HH:mm");
                }

                return default;
            }
        }
    }
}