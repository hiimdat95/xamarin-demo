using System;
using System.Collections.Generic;
using System.Text;

namespace TravellerApp.Models
{
    class review
    {
        public string traveller_partner_id { get; set; }
        public string image_medium { get; set; }
        public string rating { get; set; }
        public string total_visits { get; set; }
        public string text { get; set; }
        public DateTime date { get; set; }
        public string image_url { get; set; }

        public string dateDisplay
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(date.ToString()) && !"false".Equals(date.ToString()))
                {
                    var datefomat = date.ToString("hh MMM ");
                    var timefomat = date.ToString(" HH:mm");

                    return datefomat + "at" + timefomat;
                }

                return string.Empty;
            }
        }

        public string totalVisitedDisplay
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(total_visits) && !"false".Equals(total_visits))
                {
                    return "Visited " + total_visits + " places";
                }

                return "Visited 0 places";
            }
        }
    }
}
