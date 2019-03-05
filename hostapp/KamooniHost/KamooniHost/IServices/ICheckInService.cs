using KamooniHost.Models;
using KamooniHost.Models.Result;
using System;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface ICheckInService
    {
        Task<ToCheckInResult> GetCheckInBookings(DateTime startDate, DateTime endDate);

        Task<CheckInFormResult> GetGuestDetails(string bookingId);

        Task<TravellerFromTokenResult> GuestDetailsFromQR(string travellerToken);

        Task<GuestScanResult> ScanGuest(string guestToken);

        Task<PostResult> PostGuestDetails(CheckInFormItem checkInFormItem);

        Task<PostResult> ConfirmCheckIn(string bookingId);
    }
}