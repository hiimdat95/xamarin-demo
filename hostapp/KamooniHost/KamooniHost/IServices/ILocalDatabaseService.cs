using SQLite;

namespace KamooniHost.IServices
{
    public interface ILocalDatabaseService
    {
        SQLiteConnection GetDatabaseConnection();

        SQLiteConnection GetDatabaseConnection(string dbPath);
    }
}