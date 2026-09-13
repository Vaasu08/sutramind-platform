using Microsoft.EntityFrameworkCore;
using SutraMind.Domain.Entities;
using SutraMind.Infrastructure.Persistence;
using SutraMind.Infrastructure.Repositories;

namespace SutraMind.Tests;

public sealed class LocalPersistenceTests
{
    [Fact]
    public async Task Participant_and_outbox_are_saved_atomically()
    {
        const string databasePath = ":memory:";
        try
        {
            await using (var context = LocalDatabase.CreateContext(databasePath, "test-key"))
            {
                await LocalDatabase.InitializeAsync(context);
                var repository = new ParticipantRepository(context, Guid.NewGuid(), Guid.NewGuid());
                var participant = new Participant
                {
                    ParticipantCode = "AMV-001",
                    StudyId = Guid.NewGuid(),
                    SiteId = Guid.NewGuid(),
                    Name = "Synthetic Participant",
                    Age = 42,
                    Gender = "FEMALE",
                    ModernDiagnosis = "Rheumatoid Arthritis",
                    VyadhiCode = "V001",
                    EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow)
                };

                await repository.AddAsync(participant);

                Assert.NotNull(await repository.GetAsync(participant.Id));
                var outbox = Assert.Single(context.OutboxOperations);
                Assert.Equal(participant.Id, outbox.EntityId);
                Assert.Equal("Participant", outbox.EntityType);
                Assert.Equal("Pending", outbox.State.ToString());
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
