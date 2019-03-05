using Newtonsoft.Json;

namespace KamooniHost.Models.Profile
{
    public class Country : BaseModel
    {
        private string code;

        [JsonProperty("code")]
        public string Code { get => code; set => SetProperty(ref code, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }
    }
}