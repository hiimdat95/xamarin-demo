namespace TravellerApp.Models.Result
{
    public class UserExistCheckResult
    {
        public bool success { get; set; }

        public string message { get; set; }

        public bool user_exist { get; set; }
    }
}