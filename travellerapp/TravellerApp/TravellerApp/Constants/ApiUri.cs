namespace TravellerApp.Constants
{
    internal class  ApiUri
    {
        private const string BASE_URL_OFFICIAL = "https://kamooni.com/";
        public const string BASE_URL_TEST = "https://kamcomtest.az.kamooni.com/";
        public const string BASE_URL_TEST2 = "https://kamtest.az.kamooni.com/";

        public const string BASE_URL = BASE_URL_OFFICIAL;

        public const string CHECK_USER = "kamooni-api/CheckUser";
        public const string SIGN_IN = "kamooni-api/SignIn";
        public const string CREATE_TRAVELLER = "kamooni-api/CreateTraveller";
        public const string UPDATE_TRAVELLER = "kamooni-api/UpdateTraveller";
        public const string TRAVELLER_CLICK = "kamooni-api/TravellerClick";

        public const string FORGOT_PASSWORD = "kamooni-api/ForgotPassword";

        public const string REQUEST_CUSTOMER_CHECKED_IN = "kamooni-api/CustomerCheckedin";
        public const string FETCH_CHECKOUT_ID = "kamooni-api/FetchCheckoutId";
        public const string FETCH_ALL_HOSTS = "kamooni-api/FetchAllHosts";
        public const string AVAILABLE_ROOMS = "/surebook-api/GetAvailableRooms";
        public static string GET_HOST_DETAILS = "kamooni-api/GetHostDetails";
        public static string CREATE_BOOKING = "kamooni-api/CreateBooking";
        public static string CREATE_ACTIVITY_BOOKING = "kamooni-api/CreateActivityBooking";

        public static string GET_BOOKINGS = "kamooni-api/GetBookings";

        public static string CHECK_STATUS = "kamooni-api/CheckStats";
        public const string REVIEW_PLACE = "kamooni-api/ReviewPlace";


        public const string LOAD_ACTIVITIES = "/surebook-api/GetAvailableActivities";


        // Home
        public const string CURRENTLY_AT = "kamooni-api/CurrentlyAt";
        public const string ADD_TO_BOOKING_REQUEST = "kamooni-api/AddToBookingRequest";
        public const string ADD_TO_BOOKING_MANUALLY = "kamooni-api/AddToBookingManually";

        // Currently At
        public const string REVIEW_HOST = "kamooni-api/ReviewHost";

        public const string GET_PLACE_FOR_REVIEW = "kamooni-api/GetPlacesForReview";

        public const string GET_THINGS_TO_DO = "/surebook-api/GetThingsToDo";
        public const string REVIEW_SHARED = "kamooni-api/ReviewShared";

        public const string CHECK_OUT_TRAVELLER = "kamooni-api/CheckOutTraveller";
        public const string VIEW_BILL = "kamooni-api/ViewBill";
        public const string SEND_PAYMENT_LINK = "surebook-api/SendPaymentLink";

        //Places Visited
        public const string GET_VISITS = "kamooni-api/GetVisits";

        //Our Story
        public const string FetchTimeLine = "kamooni-api/FetchTimeLine";

        //All Posts
        public const string FetchAllPosts = "kamooni-api/FetchAllPosts";
        public const string LikeReview = "kamooni-api/LikeReview";
        public const string CommentReview = "kamooni-api/CommentReview";


        // Peach payment.
        public const string CheckoutId = "kamooni-channel/peach-mobile-get-checkout-id";
        public const string NotifyPaymentComplete = "kamooni-channel/peach-mobile-notify-payment-complete";

        //Maps
        public const string FETCH_PLACES = "kamooni-api/FetchPlaces";
        public const string FETCH_PLACE_TYPES = "kamooni-api/FetchPlaceTypes";
        public const string CREATE_PLACE = "kamooni-api/CreatePlace";
        public const string OPEN_PLACE = "kamooni-api/OpenPLace";

    }
}