using Newtonsoft.Json;

namespace KamooniHost.Models
{
    public class Host : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string email;

        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }

        private string premindPassword;

        [JsonProperty("premind_password")]
        public string PremindPassword { get => premindPassword; set => SetProperty(ref premindPassword, value); }

        private string bookingEmail;

        [JsonProperty("booking_email")]
        public string BookingEmail { get => bookingEmail; set => SetProperty(ref bookingEmail, value); }

        private string mobile;

        [JsonProperty("mobile")]
        public string Mobile { get => mobile; set => SetProperty(ref mobile, value); }

        private string street;

        [JsonProperty("street")]
        public string Street { get => street; set => SetProperty(ref street, value); }

        private string street2;

        [JsonProperty("street2")]
        public string Street2 { get => street2; set => SetProperty(ref street2, value); }

        private string city;

        [JsonProperty("city")]
        public string City { get => city; set => SetProperty(ref city, value); }

        private string zip;

        [JsonProperty("zip")]
        public string Zip { get => zip; set => SetProperty(ref zip, value); }

        private string stateCode;

        [JsonProperty("state_code")]
        public string StateCode { get => stateCode; set => SetProperty(ref stateCode, value); }

        private string stateName;

        [JsonProperty("state_name")]
        public string StateName { get => stateName; set => SetProperty(ref stateName, value); }

        private State state;

        [JsonProperty("state")]
        public State State { get => state; set => SetProperty(ref state, value); }

        private string countryCode;

        [JsonProperty("country_code")]
        public string CountryCode { get => countryCode; set => SetProperty(ref countryCode, value); }

        private string countryName;

        [JsonProperty("country_name")]
        public string CountryName { get => countryName; set => SetProperty(ref countryName, value); }

        private Country country;

        [JsonProperty("country")]
        public Country Country { get => country; set => SetProperty(ref country, value); }

        private string image;

        [JsonProperty("image")]
        public string Image { get => image; set => SetProperty(ref image, value); }

        private string url;

        [JsonProperty("host_url")]
        public string Url { get => url; set => SetProperty(ref url, value); }

        private string terms;

        [JsonProperty("terms")]
        public string Terms { get => terms; set => SetProperty(ref terms, value == "false" ? "" : value); }

        private string urlTerms;

        [JsonProperty("url_terms")]
        public string UrlTerms { get => urlTerms; set => SetProperty(ref urlTerms, value == "false" ? "" : value); }

        private string token;

        [JsonProperty("host_token")]
        public string Token { get => token; set => SetProperty(ref token, value); }

        private bool isActivity;

        [JsonProperty("is_activity")]
        public bool IsActivity { get => isActivity; set => SetProperty(ref isActivity, value); }

        private bool isAccommodation;

        [JsonProperty("is_accommodation")]
        public bool IsAccommodation { get => isAccommodation; set => SetProperty(ref isAccommodation, value); }

        private bool isTransport;

        [JsonProperty("is_transport")]
        public bool IsTransport { get => isTransport; set => SetProperty(ref isTransport, value); }

        private double latitude;

        [JsonProperty("partner_latitude")]
        public double Latitude { get => latitude; set => SetProperty(ref latitude, value); }

        private double longitude;

        [JsonProperty("partner_longitude")]
        public double Longitude { get => longitude; set => SetProperty(ref longitude, value); }

        private bool isVerified;

        [JsonProperty("is_verified")]
        public bool IsVerified { get => isVerified; set => SetProperty(ref isVerified, value); }

        private string imageUrl;

        [JsonProperty("image_url")]
        public string Image_Url { get => imageUrl; set => SetProperty(ref imageUrl, value); }
    }
}