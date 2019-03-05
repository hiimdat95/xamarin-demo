using KamooniHost.Models.Result;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface IPostsAndReviewsService
    {
        Task<HostResult> GetHostDetails();

        Task<HostResult> GetPost(string email, string password);
    }
}
