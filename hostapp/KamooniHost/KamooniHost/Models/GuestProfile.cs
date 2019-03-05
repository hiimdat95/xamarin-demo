using Newtonsoft.Json;
using Xamarin.Forms.Converters;

namespace KamooniHost.Models
{
    public class GuestProfile : BaseModel
    {
        private int id;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string gender;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("gender")]
        public string Gender { get => gender; set => SetProperty(ref gender, value); }

        private string mobile;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("mobile")]
        public string Mobile { get => mobile; set => SetProperty(ref mobile, value); }

        private string email;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }

        private string[] countryId;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("country_id")]
        public string[] CountryId { get => countryId; set => SetProperty(ref countryId, value, onChanged: () => OnPropertyChanged(nameof(CountryCode), nameof(CountryName))); }

        public string CountryCode => (CountryId != null && (CountryId.Length > 0)) ? CountryId[0] : default;

        public string CountryName => (CountryId != null && (CountryId.Length > 1)) ? CountryId[1] : default;

        private string image;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("image")]
        public string Image { get => image; set => SetProperty(ref image, value); }

        private string passportId;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("passport_id")]
        public string PassportId { get => passportId; set => SetProperty(ref passportId, value); }

        private string passportImage;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("passport_image")]
        public string PassportImage { get => passportImage; set => SetProperty(ref passportImage, value); }

        private int totalVisits;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("total_visits")]
        public int TotalVisits { get => totalVisits; set => SetProperty(ref totalVisits, value); }

        private int totalRatings;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("total_ratings")]
        public int TotalRatings { get => totalRatings; set => SetProperty(ref totalRatings, value); }

        private bool isVerified;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("is_verified")]
        public bool IsVerified { get => isVerified; set => SetProperty(ref isVerified, value); }
    }
}