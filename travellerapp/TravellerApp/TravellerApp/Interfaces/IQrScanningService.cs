using System;
using System.Threading.Tasks;
namespace TravellerApp.Interfaces
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}
