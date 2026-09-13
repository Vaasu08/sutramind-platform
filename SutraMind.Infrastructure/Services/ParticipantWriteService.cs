using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Domain.Rules;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class ParticipantWriteService(LocalDbContext context) : IParticipantWriteService
{
    public async Task WithdrawAsync(Guid participantId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var participant = await context.Participants.SingleAsync(p => p.Id == participantId, cancellationToken);

        if (!LifecycleRules.CanTransition(participant.Status, ParticipantStatus.Withdrawn))
            throw new InvalidOperationException($"Cannot withdraw a participant in status '{participant.Status}'.");

        participant.Status = ParticipantStatus.Withdrawn;
        participant.UpdatedAtUtc = DateTimeOffset.UtcNow;
        participant.LastModifiedBy = actorUserId;

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(Participant),
            EntityId = participant.Id,
            Operation = OperationType.Update,
            PayloadJson = JsonSerializer.Serialize(participant),
            ActorUserId = actorUserId,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteAsync(Guid participantId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var participant = await context.Participants.SingleAsync(p => p.Id == participantId, cancellationToken);

        if (!LifecycleRules.CanTransition(participant.Status, ParticipantStatus.Completed))
            throw new InvalidOperationException($"Cannot complete a participant in status '{participant.Status}'.");

        participant.Status = ParticipantStatus.Completed;
        participant.UpdatedAtUtc = DateTimeOffset.UtcNow;
        participant.LastModifiedBy = actorUserId;

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(Participant),
            EntityId = participant.Id,
            Operation = OperationType.Update,
            PayloadJson = JsonSerializer.Serialize(participant),
            ActorUserId = actorUserId,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
