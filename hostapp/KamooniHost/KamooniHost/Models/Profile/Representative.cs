using Newtonsoft.Json;

namespace KamooniHost.Models.Profile
{
    public class Representative : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string first_name;

        [JsonProperty("first_name")]
        public string FirstName { get => first_name; set => SetProperty(ref first_name, value); }

        private string last_name;

        [JsonProperty("last_name")]
        public string LastName { get => last_name; set => SetProperty(ref last_name, value); }

        private string email;

        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }
    }
}