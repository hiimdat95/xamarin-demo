using System;
using System.Collections.Generic;
using System.Text;

namespace TravellerApp.Models
{
    public class Places
    {
        public int id { get; set; }
        public double latitude { get; set; }
        public string name { get; set; }
        public double longitude { get; set; }
        public string place_type { get; set; }
        public double distance { get; set; }
        public string distance_display { get; set; }
        public string image { get; set; }
        public List<string> place_id { get; set; }
        public string image_url { get; set; }
    }

    public class place_id{
        public int id { get; set; }
        public string name { get; set; }
    }
}
