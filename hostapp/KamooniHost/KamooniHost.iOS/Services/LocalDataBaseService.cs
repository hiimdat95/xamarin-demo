using KamooniHost.iOS.Services;
using KamooniHost.IServices;
using SQLite;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(LocalDataBaseService))]

namespace KamooniHost.iOS.Services
{
    internal class LocalDataBaseService : ILocalDatabaseService
    {
        public static string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
        public static string dbName = "kamooni.db";

        public SQLiteConnection GetDatabaseConnection(string dbName)
        {
            return new SQLiteConnection(Path.Combine(dbPath, dbName));
        }

        public SQLiteConnection GetDatabaseConnection()
        {
            return new SQLiteConnection(Path.Combine(dbPath, dbName));
        }
    }
}