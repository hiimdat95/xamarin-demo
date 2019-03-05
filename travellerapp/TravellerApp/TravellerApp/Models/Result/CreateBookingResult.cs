namespace TravellerApp.Models.Result
{
    public class CreateBookingResult
    {
        public bool success { get; set; }

        public string message { get; set; }

        public string peach_payment_link_url { get; set; }
    }
}