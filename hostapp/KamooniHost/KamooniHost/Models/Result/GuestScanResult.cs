using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Converters;

namespace KamooniHost.Models.Result
{
    public class GuestScanResult : BaseResult
    {
        private int id;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("guest_name")]
        public string GuestName { get => name; set => SetProperty(ref name, value); }

        private string email;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }

        private string mobile;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("mobile")]
        public string Mobile { get => mobile; set => SetProperty(ref mobile, value); }

        private string country;

        [JsonConverter(typeof(IgnoreDataTypeConverter))]
        [JsonProperty("country")]
        public string Country { get => country; set => SetProperty(ref country, value); }

        private string countryName;

        public string CountryName { get => countryName; set => SetProperty(ref countryName, value); }

        private ImageSource countryFlag;

        public ImageSource CountryFlag { get => countryFlag; set => SetProperty(ref countryFlag, value); }

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