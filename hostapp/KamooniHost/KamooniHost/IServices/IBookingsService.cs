using KamooniHost.Models;
using KamooniHost.Models.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface IBookingsService
    {
        Task<AvailableRoomResult> GetAvailableRooms(DateTime startDay, DateTime endDay);

        Task<PostResult> CreateBooking(AddBooking booking);

        Task<OpenBookingsResult> GetCheckedInBookings();

        Task<OpenBookingsResult> GetCheckedOutBookings();

        Task<OpenBookingsResult> GetToArriveBookings();

        Task<CheckedInGuestsResult> GetCheckedInGuests(int bookingId);

        Task<GuestBillsResult> GetBills(int bookingId);

        Task<ExtraItemsResult> GetExtras();

        Task<PostResult> AddToBill(int bookingId, int guestId, IList<ExtraItem> extraItems);

        Task<PaymentMethodsResult> GetPaymentMethods();

        Task<PostResult> PostPayment(int bookingId, int partnerId, int method, double amount);

        Task<SendPaymentLinkResult> SendPaymentLink(int bookingId, int guestId, double amount);

        Task<PostResult> Checkout(int bookingId);
    }
}