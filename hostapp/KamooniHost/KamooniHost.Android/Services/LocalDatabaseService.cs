using KamooniHost.Droid.Services;
using KamooniHost.IServices;
using SQLite;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(LocalDatabaseService))]

namespace KamooniHost.Droid.Services
{
    public class LocalDatabaseService : ILocalDatabaseService
    {
        public static string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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