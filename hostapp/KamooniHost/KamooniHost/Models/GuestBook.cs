using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Converters;

namespace KamooniHost.Models
{
    public class GuestBook : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private List<string> travellerPartnerId;
        
        [JsonProperty("traveller_partner_id")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public List<string> TravellerPartnerId { get => travellerPartnerId; set => SetProperty(ref travellerPartnerId, value, onChanged: () => OnPropertyChanged(nameof(TravellerId), nameof(TravellerName))); }

        public string TravellerId => (TravellerPartnerId != null && (TravellerPartnerId.Count > 0)) ? TravellerPartnerId[0] : default;

        public string TravellerName => (TravellerPartnerId != null && (TravellerPartnerId.Count > 1)) ? TravellerPartnerId[1] : default;

        private string visitToken;

        [JsonProperty("visit_token")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public string VisitToken { get => visitToken; set => SetProperty(ref visitToken, value); }

        private DateTime? date;

        [JsonProperty("date")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public DateTime? Date { get => date; set => SetProperty(ref date, value); }

        private DateTime? dateOut;

        [JsonProperty("date_out")]
        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        public DateTime? DateOut { get => dateOut; set => SetProperty(ref dateOut, value, onChanged: () => OnPropertyChanged(nameof(CanCheckOut), nameof(IsCheckedOut))); }
        
        public bool CanCheckOut => !DateOut.HasValue;

        public bool IsCheckedOut => DateOut.HasValue;
    }
}