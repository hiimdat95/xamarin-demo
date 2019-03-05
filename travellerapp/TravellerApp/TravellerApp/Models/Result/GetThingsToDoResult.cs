using System;
using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    public class GetThingsToDoResult
    {
        public bool success { get; set; }

        public List<Things> things { get; set; }
    }
}