using Newtonsoft.Json;

namespace KamooniHost.Models {
	public class HostToVerify : BaseModel
    {
		private int id;

		[JsonProperty("id")]
		public int Id { get => id; set => SetProperty(ref id, value); }

		private string token;

		[JsonProperty("token")]
		public string Token { get => token; set => SetProperty(ref token, value); }

		private string name;

		[JsonProperty("name")]
		public string Name { get => name; set => SetProperty(ref name, value); }

		private string email;

		[JsonProperty("email")]
		public string Email { get => email; set => SetProperty(ref email, value); }

		private string bookingEmail;

		[JsonProperty("booking_email")]
		public string BookingEmail { get => bookingEmail; set => SetProperty(ref bookingEmail, value); }

		private string countryId;

		[JsonProperty("country_id")]
		public string CountryId { get => countryId; set => SetProperty(ref countryId, value); }

		private string stateId;

		[JsonProperty("state_id")]
		public string StateId { get => stateId; set => SetProperty(ref stateId, value); }

		private string city;

		[JsonProperty("city")]
		public string City { get => city; set => SetProperty(ref city, value); }

		private string street;

		[JsonProperty("street")]
		public string Street { get => street; set => SetProperty(ref street, value); }

		private string street2;

		[JsonProperty("street2")]
		public string Street2 { get => street2; set => SetProperty(ref street2, value); }

		private string routeId;

		[JsonProperty("route_id")]
		public string RouteId { get => routeId; set => SetProperty(ref routeId, value); }

		private string summary;

		[JsonProperty("summary")]
		public string Summary { get => summary; set => SetProperty(ref summary, value); }

		private bool isActivityCompany;

		[JsonProperty("is_activity_company")]
		public bool IsActivityCompany { get => isActivityCompany; set => SetProperty(ref isActivityCompany, value); }

		private bool isAccomodationCompany;

		[JsonProperty("is_accomodation")]
		public bool IsAccomodationCompany { get => isAccomodationCompany; set => SetProperty(ref isAccomodationCompany, value); }

		private bool isTransportCompany;

		[JsonProperty("is_transport")]
		public bool IsTransportCompany { get => isTransportCompany; set => SetProperty(ref isTransportCompany, value); }

		private string image;

		[JsonProperty("image")]
		public string Image { get => image; set => SetProperty(ref image, value); }

		private string longitude;

		[JsonProperty("partner_longitude")]
		public string Longitude { get => longitude; set => SetProperty(ref longitude, value); }

		private string latitude;

		[JsonProperty("partner_latitude")]
		public string Latitude { get => latitude; set => SetProperty(ref latitude, value); }

		private string mobile;

		[JsonProperty("mobile")]
		public string Mobile { get => mobile; set => SetProperty(ref mobile, value); }

		private string bankNumber;

		[JsonProperty("bank_account_number")]
		public string BankNumber { get => bankNumber; set => SetProperty(ref bankNumber, value); }

		private string bankCode;

		[JsonProperty("bank_code")]
		public string BankCode { get => bankCode; set => SetProperty(ref bankCode, value); }
	}
}
