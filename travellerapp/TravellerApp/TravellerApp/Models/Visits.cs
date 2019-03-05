using System;
using System.Collections.Generic;

namespace TravellerApp.Models
{
    public class Visits
    {
        public string depart_date { get; set; }

        public string summary { get; set; }

        public string URL { get; set; }

        public string arrive_date { get; set; }

        public string Host { get; set; }

        public string image_url { get; set; }

        public DateTimeOffset? date_out { get; set; }

        public bool agreed { get; set; }

        public string visit_token { get; set; }

        public DateTimeOffset? date { get; set; }

        public string Token { get; set; }

        public string booking_token { get; set; }

        public string image { get; set; }

        public DateTime arrive_dates
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(arrive_date) && !"false".Equals(arrive_date.Trim()))
                {
                    return DateTime.Parse(arrive_date);
                }

                return default;
            }
        }

        public DateTime depart_dates
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(depart_date) && !"false".Equals(depart_date.Trim()))
                {
                    return DateTime.Parse(depart_date);
                }

                return default;
            }
        }

        public string open
        {
            get
            {
                return "Open";
            }
        }

        public double partner_latitude { get; set; }

        public double partner_longitude { get; set; }

        public double distance { get; set; }

        public string distance_display { get; set; }

        public string city { get; set; }

        public string state_id { get; set; }

        public string state { get; set; }

        public string province { get; set; }

        public int id { get; set; }

    }
}
