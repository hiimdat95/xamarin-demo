using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class Customer : BaseModel
    {
        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string passportId;

        [JsonProperty("id_passport")]
        public string PassportId { get => passportId; set => SetProperty(ref passportId, value); }

        private string mobile;

        [JsonProperty("mobile")]
        public string Mobile { get => mobile; set => SetProperty(ref mobile, value); }

        private string email;

        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }
    }
}