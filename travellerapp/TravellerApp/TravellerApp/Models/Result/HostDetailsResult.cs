using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    public class HostDetailsResult
    {
        public bool success { get; set; }

        public string message { get; set; }

        public Host host { get; set; }

        public List<Reviews> reviews { get; set; }
    }
}