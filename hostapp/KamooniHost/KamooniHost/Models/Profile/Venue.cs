using Newtonsoft.Json;

namespace KamooniHost.Models.Profile
{
    public class Venue : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string city;

        [JsonProperty("city")]
        public string City { get => city; set => SetProperty(ref city, value); }

        private string nickname;

        [JsonProperty("nickname")]
        public string Nickname { get => nickname; set => SetProperty(ref nickname, value); }

        private string address;

        [JsonProperty("address")]
        public string Address { get => address; set => SetProperty(ref address, value); }
    }
}