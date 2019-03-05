using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class GuestDetailsRoom : BaseModel
    {
        private string id;

        [JsonProperty("id")]
        public string Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }
    }
}