namespace TravellerApp.Interfaces
{
    public interface IAuthService
    {
        void SaveCredentials(string UserName, string Password);

        string UserName { get; }

        string Password { get; }

        //start Premind SDK after login success 
        void onLoginSuccess(string token);
    }
}