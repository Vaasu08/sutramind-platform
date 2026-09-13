using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Repositories;

public sealed class ParticipantRepository(LocalDbContext context, Guid actorUserId, Guid deviceId) : IParticipantRepository
{
    public async Task AddAsync(Participant participant, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        context.Participants.Add(participant);
        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(Participant),
            EntityId = participant.Id,
            Operation = OperationType.Create,
            PayloadJson = JsonSerializer.Serialize(participant),
            ActorUserId = actorUserId,
            DeviceId = deviceId
        });
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public Task<Participant?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Participants.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
}
