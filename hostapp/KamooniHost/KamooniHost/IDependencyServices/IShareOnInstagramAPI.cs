using System.Threading.Tasks;

namespace KamooniHost.IDependencyServices
{
    public interface IShareOnInstagramAPI
    {
        Task<bool> OpenShareOnInstagram(string path, string content);
    }
}
