using System.Globalization;

namespace TravellerApp.Models.Result
{
    public class Host
    {
        public int id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public string mobile { get; set; }

        public string summary { get; set; }

        public string premind_password { get; set; }

        public string host_url { get; set; }

        public string host_token { get; set; }

        public string image { get; set; }

        public string booking_email { get; set; }

        public string state_code { get; set; }

        public string country_code { get; set; }

        public string street { get; set; }

        public string street2 { get; set; }

        public string city { get; set; }

        public string zip { get; set; }

        public string state_name { get; set; }

        public string country_name { get; set; }

        public string terms { get; set; }

        public string url_terms { get; set; }

        public bool is_activity_company { get; set; }

        public bool is_accommodation { get; set; }

        public bool is_transport { get; set; }

        public decimal rating { get; set; }

        public double partner_longitude { get; set; }

        public double partner_latitude { get; set; }

        public bool is_verified { get; set; }
    }
}