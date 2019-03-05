using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    public class ListBookingResult
    {
        public bool success { get; set; }

        public string message { get; set; }

        public List<Bookings> bookings { get; set; }
    }
}