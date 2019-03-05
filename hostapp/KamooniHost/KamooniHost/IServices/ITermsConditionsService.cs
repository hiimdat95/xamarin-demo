using KamooniHost.Models;
using KamooniHost.Models.Result;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface ITermsConditionsService
    {
        Task<PostResult> NewTermsConditions(Host Host);
    }
}