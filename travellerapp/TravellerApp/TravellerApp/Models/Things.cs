using System;
using System.Collections.Generic;

namespace TravellerApp.Models
{
    public class Things
    {
        public string list_price { get; set; }

        public string name { get; set; }

        public string image { get; set; }

        public string image_url { get; set; }

        public string summary { get; set; }

        public int sales_count { get; set; }

        public int id { get; set; }

        public string price_display
        {
            get
            {
                if (!"0.00".Equals(list_price) && !"0.0".Equals(list_price))
                {
                    return "R" + " " + list_price;
                }

                return "Free";
            }
        }
    }
}