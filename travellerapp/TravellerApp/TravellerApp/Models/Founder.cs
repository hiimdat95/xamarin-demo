using System;

namespace TravellerApp.Models
{
    public class Founder
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int total_visits { get; set; }
        public string image_url { get; set; }
        public string image_medium { get; set; }
        public string traveller_partner_id { get; set; }
        public string text { get; set; }
        public DateTime date { get; set; }
        public string rating { get; set; }
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
        public bool visibleImage
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(image_url) && !"false".Equals(image_url))
                {
                    return true;
                }

                return false;
            }
        }

        public string name { get; set; }
    }
}
