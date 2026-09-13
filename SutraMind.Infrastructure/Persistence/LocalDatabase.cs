using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace SutraMind.Infrastructure.Persistence;

public static class LocalDatabase
{
    private static int initialized;

    public static LocalDbContext CreateContext(string databasePath, string encryptionKey)
    {
        Batteries_V2.Init();
        Interlocked.Exchange(ref initialized, 1);
        var connection = new SqliteConnection($"Data Source={databasePath};Mode=ReadWriteCreate;Cache=Shared;Password={encryptionKey}");
        connection.Open();
        var options = new DbContextOptionsBuilder<LocalDbContext>()
            .UseSqlite(connection)
            .Options;
        return new LocalDbContext(options);
    }

    public static async Task InitializeAsync(LocalDbContext context, CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);
    }
}
