using System;

namespace TravellerApp.Models.Result
{
    public class Reviews
    {                            
        public string traveller_partner_id { get; set; }

        public string image_medium { get; set; }

        public DateTime date { get; set; }

        public string rating { get; set; }

        public string text { get; set; }

        public int total_visits { get; set; }

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
    }
}