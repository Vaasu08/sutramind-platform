using SutraMind.Application.Abstractions;
using SutraMind.Application.Services;
using SutraMind.Domain.Entities;

namespace SutraMind.Tests;

public sealed class ParticipantServiceTests
{
    [Fact]
    public async Task EnrollAsync_creates_pending_participant()
    {
        var repository = new InMemoryParticipantRepository();
        var service = new ParticipantService(repository);

        var participant = await service.EnrollAsync(
            " P-001 ",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sandhivata",
            "VYADHI_001");

        Assert.Equal("P-001", participant.ParticipantCode);
        Assert.Equal("VYADHI_001", participant.VyadhiCode);
        Assert.Equal(Domain.Enums.SyncState.Pending, participant.SyncState);
        Assert.Same(participant, await repository.GetAsync(participant.Id));
    }

    private sealed class InMemoryParticipantRepository : IParticipantRepository
    {
        private readonly Dictionary<Guid, Participant> store = [];

        public Task AddAsync(Participant participant, CancellationToken cancellationToken = default)
        {
            store.Add(participant.Id, participant);
            return Task.CompletedTask;
        }

        public Task<Participant?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            store.TryGetValue(id, out var participant);
            return Task.FromResult(participant);
        }
    }
}
