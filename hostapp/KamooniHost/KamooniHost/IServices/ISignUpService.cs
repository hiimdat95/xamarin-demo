using KamooniHost.Models;
using KamooniHost.Models.Result;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface ISignUpService
    {
        Task<HostDirectoryResult> GetHostDirectory();

        Task<SuccessCheckResult> SendPortalInvite(int hostId);

        Task<HostTokenResult> CreateHost(HostToCreate hostToCreate);

        Task<CheckHostVerifyResult> CheckHostVerify(string hostToken);
    }
}