using System;
using System.Collections.Generic;
using System.Text;

namespace KamooniHost.Models
{
    public class Posts
    {
        public DateTime date { get; set; }
        public string host_partner_id { get; set; }
        public string host_partner_name { get; set; }
        public string image_url { get; set; }
        public string place_of_interest_id { get; set; }
        public string place_of_interest_name { get; set; }
        public string rating { get; set; }
        public string rating_token { get; set; }
        public string text { get; set; }
        public string total_visits { get; set; }
        public string traveller_partner_id { get; set; }
        public string traveller_partner_name { get; set; }
        public string traveller_partner_profile { get; set; }
        
        public bool VisibleImage
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(traveller_partner_profile) && !"false".Equals(traveller_partner_profile))
                {
                    return true;
                }

                return false;
            }
        }

        public string Date_Display
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
