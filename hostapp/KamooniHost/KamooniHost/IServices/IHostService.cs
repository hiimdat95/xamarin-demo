using KamooniHost.Models;
using KamooniHost.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface IHostService
    {
        Task<List<string>> GetListDatabase(string url);

        Task<HostTokenResult> GetHostToken(string url, string db, string user, string password);

        Task<HostResult> GetHostDetails(string token);

        Task<PostResult> UpdateHost(Host host);
    }
}