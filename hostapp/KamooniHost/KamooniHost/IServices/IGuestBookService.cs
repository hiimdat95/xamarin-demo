using KamooniHost.Models;
using KamooniHost.Models.Result;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface IGuestBookService
    {
        Task<GuestScanResult> ScanGuest(string guestToken);

        Task<GuestScanResult> ManualCheckIn(GuestCheckIn guest);

        Task<GuestBookResult> GetGuestBooks();

        Task<GuestProfileResult> GetGuestProfile(int id);

        Task<PostResult> GuestCheckOut(string guestToken);

        Task<PostResult> DownVoteGuest(string guestToken, string note);

        Task<PostResult> UpVoteGuest(string guestToken);
    }
}