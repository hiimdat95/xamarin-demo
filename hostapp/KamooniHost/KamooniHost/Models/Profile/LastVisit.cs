using Newtonsoft.Json;
using System;

namespace KamooniHost.Models.Profile
{
    public class LastVisit : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private DateTimeOffset? timestamp;

        [JsonProperty("timestamp")]
        public DateTimeOffset? Timestamp { get => timestamp; set => SetProperty(ref timestamp, value); }

        private DateTimeOffset? exit_time;

        [JsonProperty("exit_time")]
        public DateTimeOffset? ExitTime { get => exit_time; set => SetProperty(ref exit_time, value); }

        private int duration;

        [JsonProperty("duration")]
        public int Duration { get => duration; set => SetProperty(ref duration, value); }

        private string human_duration;

        [JsonProperty("human_duration")]
        public string HumanDuration { get => human_duration; set => SetProperty(ref human_duration, value); }

        private Venue venue = new Venue();

        [JsonProperty("venue")]
        public Venue Venue { get => venue; set => SetProperty(ref venue, value); }
    }
}