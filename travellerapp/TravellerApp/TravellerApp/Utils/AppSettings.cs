using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace TravellerApp
{
    public static class AppSettings
    {
        private const string LOGGED_IN_KEY = "LOGGED_IN_KEY";
        /*
        public static bool IsLoggedIn() {
            if (App.Current.Properties.ContainsKey(LOGGED_IN_KEY)) {
                bool loggedIn = (bool)App.Current.Properties[LOGGED_IN_KEY];
                if (loggedIn) {
                    return true;
                } else {
                    return false;
                }
            }
            return false;
        }

        public static void SetLoggedIn(bool loggedIn) {
            App.Current.Properties[LOGGED_IN_KEY] = loggedIn;
        }

        */

        private static ISettings Settings
        {
            get
            {
                if (CrossSettings.IsSupported)
                    return CrossSettings.Current;

                return null; // or your custom implementation
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion Setting Constants

        public static string GeneralSettings
        {
            get => Settings.GetValueOrDefault(SettingsKey, SettingsDefault);
            set => Settings.AddOrUpdateValue(SettingsKey, value);
        }

        public static int RequestTimeOut
        {
            get => Settings.GetValueOrDefault(nameof(RequestTimeOut), 20);
            set => Settings.AddOrUpdateValue(nameof(RequestTimeOut), value);
        }

        private static bool IsRequestTimeOutSet => Settings.Contains(nameof(RequestTimeOut));

        public static void RemoveRequestTimeOut() => Settings.Remove(nameof(RequestTimeOut));

        public static bool LoggedIn
        {
            get => Settings.GetValueOrDefault(nameof(LoggedIn), false);
            set => Settings.AddOrUpdateValue(nameof(LoggedIn), value);
        }

        private static bool IsLoggedInSet => Settings.Contains(nameof(LoggedIn));

        public static void RemoveLoggedIn() => Settings.Remove(nameof(LoggedIn));

        public static long HostID
        {
            get => Settings.GetValueOrDefault(nameof(HostID), 0);
            set => Settings.AddOrUpdateValue(nameof(HostID), value);
        }

        private static bool IsHostIDSet => Settings.Contains(nameof(HostID));

        public static void RemoveHostID() => Settings.Remove(nameof(HostID));

        public static string Password
        {
            get => Settings.GetValueOrDefault(nameof(Password), string.Empty);
            set => Settings.AddOrUpdateValue(nameof(Password), value);
        }

        private static bool IsPasswordSet => Settings.Contains(nameof(Password));

        public static void RemovePassword() => Settings.Remove(nameof(Password));

        //public static Host CurrentHost
        //{
        //    get
        //    {
        //        Host host = null;
        //        string value = AppSettings.GetValueOrDefault(nameof(CurrentHost), string.Empty);
        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            host = JsonConvert.DeserializeObject<Host>(value);
        //        }
        //        return host;
        //    }
        //    set => AppSettings.AddOrUpdateValue(nameof(CurrentHost), JsonConvert.SerializeObject(value));
        //}

        //static bool IsCurrentHostSet => AppSettings.Contains(nameof(CurrentHost));

        //public static void RemoveCurrentHost() => AppSettings.Remove(nameof(CurrentHost));

        public static string SessionToken
        {
            get => Settings.GetValueOrDefault(nameof(SessionToken), string.Empty);
            set => Settings.AddOrUpdateValue(nameof(SessionToken), value);
        }

        private static bool IsSessionTokenSet => Settings.Contains(nameof(SessionToken));

        public static void RemoveSessionToken() => Settings.Remove(nameof(SessionToken));

        public static void ClearEverything()
        {
            Settings.Clear();
        }
    }
}