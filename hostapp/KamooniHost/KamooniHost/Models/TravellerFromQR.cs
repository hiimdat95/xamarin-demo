using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class TravellerFromQR : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string email;

        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }

        private string mobile;

        [JsonProperty("mobile")]
        public string Mobile { get => mobile; set => SetProperty(ref mobile, value); }

        private string passportId;

        [JsonProperty("passport_id")]
        public string PassportId { get => passportId; set => SetProperty(ref passportId, value); }

        private string room;

        [JsonProperty("room")]
        public string Room { get => room; set => SetProperty(ref room, value); }

        private string countryISO;

        [JsonProperty("country")]
        public string CountryISO { get => countryISO; set => SetProperty(ref countryISO, value); }
    }
}