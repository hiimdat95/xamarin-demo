using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class BookingsService : IBookingsService
    {
        private readonly IRestClient restClient;

        public BookingsService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<AvailableRoomResult> GetAvailableRooms(DateTime startDay, DateTime endDay)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("source", new JObject()
                    {
                        new JProperty("source", "Kamooni Host App"),
                        new JProperty("channel", "Kamooni Host App")
                    }),
                    new JProperty("dates", new JObject()
                    {
                        new JProperty("start_date", startDay.ToString("yyyy-M-d")),
                        new JProperty("end_date", endDay.ToString("yyyy-M-d"))
                    })
                })
            };

            return await restClient.PostAsync<AvailableRoomResult, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_AVAILABLE_ROOMS, @params);
        }

        public async Task<PostResult> CreateBooking(AddBooking booking)
        {
            JArray roomdata = new JArray();
            foreach (var r in booking.ListRoom)
            {
                roomdata.Add(new JObject()
                {
                    new JProperty("id", r.Id),
                    new JProperty("type", r.Type),
                    new JProperty("room_type", r.RoomType),
                    new JProperty("start_date", booking.CheckInDate),
                    new JProperty("end_date", booking.CheckOutDate),
                    new JProperty("guest_total", r.TotalGuest)
                });
            }

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("source", new JObject()
                    {
                        new JProperty("source", "Kamooni Host App"),
                        new JProperty("channel", "Kamooni Host App")
                    }),
                    new JProperty("usr_token", "False"),
                    new JProperty("customer", new JObject()
                    {
                        new JProperty("name", booking.Customer.Name),
                        new JProperty("email", booking.Customer.Email),
                        new JProperty("mobile", booking.Customer.Mobile)
                    }),
                    new JProperty("roomdata", roomdata)
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_CREATE_BOOKING, @params);

            return response?.Result;
        }

        public async Task<OpenBookingsResult> GetCheckedInBookings()
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    })
                })
            };

            CheckedInBookingsResponse response = await restClient.PostAsync<CheckedInBookingsResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_CHECKED_IN_BOOKINGS, @params);

            return response?.Result;
        }

        public async Task<OpenBookingsResult> GetCheckedOutBookings()
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    })
                })
            };

            CheckedInBookingsResponse response = await restClient.PostAsync<CheckedInBookingsResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_CHECKED_OUT_BOOKINGS, @params);

            return response?.Result;
        }

        public async Task<OpenBookingsResult> GetToArriveBookings()
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    })
                })
            };

            CheckedInBookingsResponse response = await restClient.PostAsync<CheckedInBookingsResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_TO_ARRIVE_BOOKINGS, @params);

            return response?.Result;
        }

        public async Task<CheckedInGuestsResult> GetCheckedInGuests(int bookingId)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("booking", new JObject()
                    {
                        new JProperty("booking_id", bookingId)
                    })
                })
            };

            CheckedInGuestsResponse response = await restClient.PostAsync<CheckedInGuestsResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_CHECKED_IN_GUESTS, @params);

            return response?.Result;
        }

        public async Task<GuestBillsResult> GetBills(int bookingId)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("booking", new JObject()
                    {
                        new JProperty("booking_id", bookingId)
                    })
                })
            };

            GuestBillsResponse response = await restClient.PostAsync<GuestBillsResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_CHECKED_IN_BOOKING_DETAIL, @params);

            return response?.Result;
        }

        public async Task<ExtraItemsResult> GetExtras()
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    })
                })
            };

            ExtraItemsResponse response = await restClient.PostAsync<ExtraItemsResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_EXTRA_ITEMS, @params);

            return response?.Result;
        }

        public async Task<PostResult> AddToBill(int bookingId, int guestId, IList<ExtraItem> extraItems)
        {
            JArray extra_items = new JArray();
            foreach (var item in extraItems)
            {
                extra_items.Add(new JObject()
                {
                    new JProperty("product_id", item.Id),
                    new JProperty("uom_id", item.UomId),
                    new JProperty("qty", item.Quantity)
                });
            }

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("booking", new JObject()
                    {
                        new JProperty("booking_id", bookingId)
                    }),
                    new JProperty("guest", new JObject()
                    {
                        new JProperty("guest_id", guestId)
                    }),
                    new JProperty("extra_items", extra_items)
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_ADD_EXTRA_ITEMS, @params);

            return response?.Result;
        }

        public async Task<PaymentMethodsResult> GetPaymentMethods()
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    })
                })
            };

            PaymentMethodsResponse response = await restClient.PostAsync<PaymentMethodsResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_PAYMENT_METHODS, @params);

            return response?.Result;
        }

        public async Task<PostResult> PostPayment(int bookingId, int partnerId, int method, double amount)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("payment", new JObject()
                    {
                        new JProperty("booking_id", bookingId),
                        new JProperty("paid_for_partner_id", partnerId),
                        new JProperty("method", method),
                        new JProperty("amount", amount)
                    })
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_POST_PAYMENT, @params);

            return response?.Result;
        }

        public async Task<SendPaymentLinkResult> SendPaymentLink(int bookingId, int guestId, double amount)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("host_token", Settings.CurrentHost.Token),
                        new JProperty("booking_id", bookingId)
                    }),
                    new JProperty("payment", new JObject()
                    {
                        new JProperty("guest_id", guestId),
                        new JProperty("amount", amount)
                    })
                })
            };

            SendPaymentLinkResponse response = await restClient.PostAsync<SendPaymentLinkResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.SEND_PAYMENT_LINK, @params);

            return response?.Result;
        }

        public async Task<PostResult> Checkout(int bookingId)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("booking", new JObject()
                    {
                        new JProperty("booking_id", bookingId)
                    })
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_CHECK_OUT, @params);

            return response?.Result;
        }
    }
}