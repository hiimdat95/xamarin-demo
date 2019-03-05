using KamooniHost.Models.Result;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface IStatsService
    {
        Task<RevenueResult> GetRevenue();
    }
}