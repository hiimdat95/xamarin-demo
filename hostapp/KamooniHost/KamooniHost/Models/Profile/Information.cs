using Newtonsoft.Json;
using System;

namespace KamooniHost.Models.Profile
{
    public class Information : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string key;

        [JsonProperty("key")]
        public string Key { get => key; set => SetProperty(ref key, value); }

        private string value;

        [JsonProperty("value")]
        public string Value { get => value; set => SetProperty(ref this.value, value); }

        private Representative representative = new Representative();

        [JsonProperty("representative")]
        public Representative Representative { get => representative; set => SetProperty(ref representative, value); }

        private Venue venue = new Venue();

        [JsonProperty("venue")]
        public Venue Venue { get => venue; set => SetProperty(ref venue, value); }

        private bool visible;

        [JsonProperty("visible")]
        public bool Visible { get => visible; set => SetProperty(ref visible, value); }

        private bool @internal;

        [JsonProperty("internal")]
        public bool Internal { get => @internal; set => SetProperty(ref @internal, value); }

        private DateTimeOffset date_added;

        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get => date_added; set => SetProperty(ref date_added, value); }

        private DateTimeOffset date_changed;

        [JsonProperty("date_changed")]
        public DateTimeOffset DateChanged { get => date_changed; set => SetProperty(ref date_changed, value); }

        private int profile_id;

        [JsonProperty("profile_id")]
        public int ProfileId { get => profile_id; set => SetProperty(ref profile_id, value); }

        private string external_id;

        [JsonProperty("external_id")]
        public string ExternalId { get => external_id; set => SetProperty(ref external_id, value); }
    }
}