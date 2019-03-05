using KamooniHost.Models.Profile.Auth;
using KamooniHost.Models.Result;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface ILoginService
    {
        Task<UserExistCheckResult> CheckIfExist(string email);

        Task<HostResult> Login(string email, string password);

        Task<string> PremindLogin(string email, string password);

        Task<SuccessCheckResult> ForgotPassword(string email);
    }
}