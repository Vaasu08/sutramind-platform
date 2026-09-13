using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace SutraMind.Infrastructure.Persistence;

public static class LocalDatabase
{
    private static int initialized;

    public static LocalDbContext CreateContext(string databasePath, string encryptionKey)
    {
        if (Interlocked.Exchange(ref initialized, 1) == 0)
            Batteries_V2.Init();

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Password = encryptionKey
        };

        var options = new DbContextOptionsBuilder<LocalDbContext>()
            .UseSqlite(builder.ConnectionString)
            .Options;
        return new LocalDbContext(options);
    }

    public static async Task InitializeAsync(LocalDbContext context, CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);
    }
}
