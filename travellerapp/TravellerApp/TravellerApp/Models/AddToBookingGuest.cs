using Newtonsoft.Json;

namespace TravellerApp.Models
{
    public class AddToBookingGuest : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string product_id;

        [JsonProperty("product_id")]
        public string ProductId { get => product_id; set => SetProperty(ref product_id, value); }

        private string customer;

        [JsonProperty("customer")]
        public string Customer { get => customer; set => SetProperty(ref customer, value); }

        private string email;

        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }

        public string TextColor => string.IsNullOrWhiteSpace(Email) ? "White" : "Gray";
    }
}