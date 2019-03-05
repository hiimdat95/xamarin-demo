using Newtonsoft.Json;

namespace KamooniHost.Models.Profile.Auth
{
    public class Auth : BaseModel
    {
        private object[] exclude_membership_levels;

        [JsonProperty("exclude_membership_levels")]
        public object[] ExcludeMembershipLevels { get => exclude_membership_levels; set => SetProperty(ref exclude_membership_levels, value); }

        private string token;

        [JsonProperty("token")]
        public string Token { get => token; set => SetProperty(ref token, value); }

        private string group_name;

        [JsonProperty("group_name")]
        public string GroupName { get => group_name; set => SetProperty(ref group_name, value); }

        private int venue_id;

        [JsonProperty("venue_id")]
        public int VenueId { get => venue_id; set => SetProperty(ref venue_id, value); }

        private string currency;

        [JsonProperty("currency")]
        public string Currency { get => currency; set => SetProperty(ref currency, value); }

        private string currency_human;

        [JsonProperty("currency_human")]
        public string CurrencyHuman { get => currency_human; set => SetProperty(ref currency_human, value); }

        private string host;

        [JsonProperty("host")]
        public string Host { get => host; set => SetProperty(ref host, value); }

        private string scheme;

        [JsonProperty("scheme")]
        public string Scheme { get => scheme; set => SetProperty(ref scheme, value); }

        private string ws_scheme;

        [JsonProperty("ws_scheme")]
        public string WsScheme { get => ws_scheme; set => SetProperty(ref ws_scheme, value); }
    }
}