using System.IO;

namespace TravellerApp.Interfaces
{
    public interface IBarcodeService
    {
        Stream GenerateQR(string text, int width = 300, int height = 300);
    }
}