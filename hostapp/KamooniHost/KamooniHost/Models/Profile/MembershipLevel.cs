using Newtonsoft.Json;

namespace KamooniHost.Models.Profile
{
    public class MembershipLevel : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private int ordering;

        [JsonProperty("ordering")]
        public int Ordering { get => ordering; set => SetProperty(ref ordering, value); }
    }
}