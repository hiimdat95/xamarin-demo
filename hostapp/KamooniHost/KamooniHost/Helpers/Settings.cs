using KamooniHost.Models;
using Newtonsoft.Json;
using Plugin.Settings.Abstractions;
using System.Collections.Generic;

namespace KamooniHost.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters.
    /// </summary>
    public static class Settings
    {
        private static ISettings CrossSettings => Plugin.Settings.CrossSettings.IsSupported ? Plugin.Settings.CrossSettings.Current : null;

        #region LoggedIn

        public static bool LoggedIn
        {
            get => CrossSettings.GetValueOrDefault(nameof(LoggedIn), false);
            set => CrossSettings.AddOrUpdateValue(nameof(LoggedIn), value);
        }

        private static bool IsLoggedInSet => CrossSettings.Contains(nameof(LoggedIn));

        public static void RemoveLoggedIn() => CrossSettings.Remove(nameof(LoggedIn));

        #endregion LoggedIn

        #region HostID

        public static long HostID
        {
            get => CrossSettings.GetValueOrDefault(nameof(HostID), 0);
            set => CrossSettings.AddOrUpdateValue(nameof(HostID), value);
        }

        private static bool IsHostIDSet => CrossSettings.Contains(nameof(HostID));

        public static void RemoveHostID() => CrossSettings.Remove(nameof(HostID));

        #endregion HostID

        #region Email

        public static string Email
        {
            get => CrossSettings.GetValueOrDefault(nameof(Email), string.Empty);
            set => CrossSettings.AddOrUpdateValue(nameof(Email), value);
        }

        private static bool IsEmailSet => CrossSettings.Contains(nameof(Email));

        public static void RemoveEmail() => CrossSettings.Remove(nameof(Email));

        #endregion Email

        #region PIN

        public static string PIN
        {
            get => CrossSettings.GetValueOrDefault(nameof(PIN), string.Empty);
            set => CrossSettings.AddOrUpdateValue(nameof(PIN), value);
        }

        private static bool IsPINSet => CrossSettings.Contains(nameof(PIN));

        public static void RemovePIN() => CrossSettings.Remove(nameof(PIN));

        #endregion PIN


        #region Password

        public static string Password
        {
            get => CrossSettings.GetValueOrDefault(nameof(Password), string.Empty);
            set => CrossSettings.AddOrUpdateValue(nameof(Password), value);
        }

        private static bool IsPasswordSet => CrossSettings.Contains(nameof(Password));

        public static void RemovePassword() => CrossSettings.Remove(nameof(Password));

        #endregion Password

        #region ListHost

        public static List<Host> ListHost
        {
            get
            {
                string value = CrossSettings.GetValueOrDefault(nameof(ListHost), string.Empty);
                return (string.IsNullOrWhiteSpace(value) ? null : JsonConvert.DeserializeObject<List<Host>>(value)) ?? new List<Host>();
            }
            set => CrossSettings.AddOrUpdateValue(nameof(ListHost), JsonConvert.SerializeObject(value));
        }

        private static bool IsListHostSet => CrossSettings.Contains(nameof(ListHost));

        public static void RemoveListHost() => CrossSettings.Remove(nameof(ListHost));

        #endregion ListHost

        #region CurrentHost

        public static Host CurrentHost
        {
            get
            {
                string value = CrossSettings.GetValueOrDefault(nameof(CurrentHost), string.Empty);
                return string.IsNullOrWhiteSpace(value) ? null : JsonConvert.DeserializeObject<Host>(value);
            }
            set => CrossSettings.AddOrUpdateValue(nameof(CurrentHost), JsonConvert.SerializeObject(value));
        }

        private static bool IsCurrentHostSet => CrossSettings.Contains(nameof(CurrentHost));

        public static void RemoveCurrentHost() => CrossSettings.Remove(nameof(CurrentHost));

        #endregion CurrentHost

        #region CurrentPost

        public static List<Post> CurrentPost
        {
            get
            {
                string value = CrossSettings.GetValueOrDefault(nameof(CurrentPost), string.Empty);
                return string.IsNullOrWhiteSpace(value) ? null : JsonConvert.DeserializeObject<List<Post>>(value);
            }
            set => CrossSettings.AddOrUpdateValue(nameof(CurrentPost), JsonConvert.SerializeObject(value));
        }

        private static bool IsCurrentPostSet => CrossSettings.Contains(nameof(CurrentPost));

        public static void RemoveCurrentPost() => CrossSettings.Remove(nameof(CurrentPost));

        #endregion

        #region PremindUri

        public static string PremindUri
        {
            get => CrossSettings.GetValueOrDefault(nameof(PremindUri), string.Empty);
            set => CrossSettings.AddOrUpdateValue(nameof(PremindUri), value);
        }

        private static bool IsPremindUriSet => CrossSettings.Contains(nameof(PremindUri));

        public static void RemovePremindUri() => CrossSettings.Remove(nameof(PremindUri));

        #endregion PremindUri

        #region SessionToken

        public static string SessionToken
        {
            get => CrossSettings.GetValueOrDefault(nameof(SessionToken), string.Empty);
            set => CrossSettings.AddOrUpdateValue(nameof(SessionToken), value);
        }

        private static bool IsSessionTokenSet => CrossSettings.Contains(nameof(SessionToken));

        public static void RemoveSessionToken() => CrossSettings.Remove(nameof(SessionToken));

        #endregion SessionToken

        #region AppSettings

        public static AppSettings AppSettings
        {
            get
            {
                string value = CrossSettings.GetValueOrDefault(nameof(AppSettings), string.Empty);
                return (string.IsNullOrWhiteSpace(value) ? null : JsonConvert.DeserializeObject<AppSettings>(value)) ?? new AppSettings();
            }
            set => CrossSettings.AddOrUpdateValue(nameof(AppSettings), JsonConvert.SerializeObject(value));
        }

        private static bool IsAppSettingsSet => CrossSettings.Contains(nameof(AppSettings));

        public static void RemoveAppSettings() => CrossSettings.Remove(nameof(AppSettings));

        #endregion AppSettings

        public static void ClearEverything()
        {
            CrossSettings.Clear();
        }
    }
}