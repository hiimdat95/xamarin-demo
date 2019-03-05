namespace TravellerApp.Models.Result
{
    public class UserResult
    {
        public bool success { get; set; }

        public string message { get; set; }

        public User traveller { get; set; }
    }
}