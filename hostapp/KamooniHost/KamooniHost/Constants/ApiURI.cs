namespace KamooniHost.Constants
{
    public class ApiURI
    {
        private static readonly string URL_MAIN_OFFICIAL = "https://kamooni.com";
        private static readonly string URL_MAIN_TEST = "https://kamcomtest.az.kamooni.com";

        public static string URL_MAIN = URL_MAIN_OFFICIAL;

        public static string URI_GET_DATABASE_LIST = "/web/database/list";

        // WEB SOCKET //
        public static string URI_WEBSOCKET = "https://pre-mind.com/api";

        public static string URI_WEBSOCKET_LOGIN = "/api-auth/";
        public static string URI_WEBSOCKET_PROFILE = "/profile/";

        // LOGIN //
        public static string URI_CHECK_USER = "/kamooni-host-api/CheckUser";

        public static string URI_SIGN_IN = "/kamooni-host-api/SignIn";
        public static string URI_FORGOT_PASSWORD = "/kamooni-host-api/ForgotPassword";

        // HOST //
        public static string URI_GET_HOST_DIRECTORY = "/kamooni-host-api/ReturnAllHosts";

        public static string URI_SEND_PORTAL_INVITE = "/kamooni-host-api/SendPortalInvite";
        public static string URI_CREATE_HOST = "/kamooni-host-api/CreateHost";
        public static string URI_CHECK_HOST_VERIFY = "/kamooni-host-api/CheckHostVerify";
        public static string URI_VERIFY_HOST_DETAILS = "/kamooni-host-api/VerifyHostDetails";
        public static string URI_UPDATE_HOST = "/kamooni-host-api/UpdateHost";
        public static string URI_GET_HOST_DETAILS = "/kamooni-api/GetHostDetails";
        public static string URI_GET_HOST_TOKEN = "/surebook-api/GetHostToken";

        // CHECK IN //
        public static string URI_FETCH_BOOKING = "/surebook-api/FetchBookings";

        public static string URI_GET_CHECK_IN_FORM = "/surebook-api/GetCheckInForm";
        public static string URI_UPDATE_CHECK_IN_FORM = "/surebook-api/UpdateCheckInForm";
        public static string URI_CONFIRM_CHECK_IN = "/surebook-api/ConfirmCheckIn";

        // OPEN BOOKINGS //
        public static string URI_GET_CHECKED_IN_BOOKINGS = "/surebook-api/GetCheckedInBookings";

        public static string URI_GET_CHECKED_OUT_BOOKINGS = "/surebook-api/GetCheckedOutBookings";
        public static string URI_GET_TO_ARRIVE_BOOKINGS = "/surebook-api/GetToArriveBookings";
        public static string URI_GET_CHECKED_IN_BOOKING_DETAIL = "/surebook-api/GetCheckedInBookingDetail";
        public static string URI_GET_CHECKED_IN_BOOKING_PAY_NOW = "/surebook-api/GetCheckedInBookingPayNow";
        public static string URI_GET_PAYMENT_METHODS = "/surebook-api/GetPaymentMethods";
        public static string URI_POST_PAYMENT = "/surebook-api/PostPayment";
        public static string URI_GET_CHECKED_IN_GUESTS = "/surebook-api/GetCheckedInGuests";
        public static string URI_GET_EXTRA_ITEMS = "/surebook-api/GetExtraItems";
        public static string URI_ADD_EXTRA_ITEMS = "/surebook-api/AddExtraItems";
        public static string URI_GET_REVENUE = "/surebook-api/GetRevenue";
        public static string URI_CHECK_OUT = "/surebook-api/CheckOut";
        public static string SEND_PAYMENT_LINK = "/surebook-api/SendPaymentLink";

        // BOOKING //
        public static string URI_CREATE_BOOKING = "/surebook-api/CreateBooking";

        public static string URI_GET_AVAILABLE_ROOMS = "/surebook-api/GetAvailableRooms";

        // GUEST BOOK //
        public static string GUEST_BOOK_SCAN_GUEST = "/kamooni-host-api/ScanGuest";

        public static string MANUAL_CHECK_IN = "/kamooni-host-api/ManualCheckIn";
        public static string GET_GUEST_BOOK = "/kamooni-host-api/RertunGuestBook";
        public static string GET_GUEST_PROFILE = "/kamooni-host-api/OpenGuestProfile";
        public static string GUEST_BOOK_CHECK_OUT = "/kamooni-host-api/CheckOut";
        public static string GUEST_DOWN_VOTE = "/kamooni-host-api/DownVoteGuest";
        public static string GUEST_UP_VOTE = "/kamooni-host-api/UpVoteGuest";

        // TRAVELLER //
        public static string GET_TRAVELLER_FROM_TOKEN = "/kamooni-api/GetTravellerFromToken";

        // COUNTRY SERVICE //
        public static string URI_GET_STATES = "/kamooni-host-api/ReturnStates";

        // TERMS AND CONDITIONS//
        public static string UPDATE_TERMS = "/kamooni-host-api/UpdateTerms";

        public const string FETCH_PLACE_TYPES = "/kamooni-api/FetchPlaceTypes";
        public const string REVIEW_HOST = "/kamooni-api/ReviewHost";
        public const string REVIEW_SHARED = "/kamooni-api/ReviewShared";
        public const string FETCH_POSTS_FOR_HOST = "/kamooni-api/FetchPostsForHost";
    }
}