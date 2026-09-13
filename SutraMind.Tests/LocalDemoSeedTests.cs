using Microsoft.EntityFrameworkCore;
using SutraMind.Infrastructure.Persistence;
using SutraMind.Infrastructure.Seed;

namespace SutraMind.Tests;

public sealed class LocalDemoSeedTests
{
    [Fact]
    public async Task EnsureSeededAsync_is_idempotent()
    {
        const string databasePath = ":memory:";
        try
        {
            await using (var context = LocalDatabase.CreateContext(databasePath, "seed-test-key"))
            {
                await LocalDatabase.InitializeAsync(context);
                await LocalDemoSeed.EnsureSeededAsync(context);
                var firstCount = await context.Participants.CountAsync();
                await LocalDemoSeed.EnsureSeededAsync(context);
                var secondCount = await context.Participants.CountAsync();
                Assert.Equal(firstCount, secondCount);
                Assert.True(firstCount >= 5);
                context.Database.GetDbConnection().Close();
            }
        }
        finally
        {
            if (File.Exists(databasePath)) File.Delete(databasePath);
            if (File.Exists($"{databasePath}-wal")) File.Delete($"{databasePath}-wal");
            if (File.Exists($"{databasePath}-shm")) File.Delete($"{databasePath}-shm");
        }
    }
}
