using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    public class ListInfoResult
    {
        public int found { get; set; }

        public bool success { get; set; }

        public List<ListInfo> hosts { get; set; }
    }
}