using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class VisitWriteService(LocalDbContext context) : IVisitWriteService
{
    public async Task CompleteAsync(Guid visitId, DateOnly visitDate, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var visit = await context.Visits.SingleAsync(v => v.Id == visitId, cancellationToken);

        if (visit.Status != VisitStatus.Scheduled)
            throw new InvalidOperationException($"Only scheduled visits can be completed. Current status: {visit.Status}.");

        visit.Status = VisitStatus.Completed;
        visit.VisitDate = visitDate;
        visit.UpdatedAtUtc = DateTimeOffset.UtcNow;
        visit.LastModifiedBy = actorUserId;

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(Visit),
            EntityId = visit.Id,
            Operation = OperationType.Update,
            PayloadJson = JsonSerializer.Serialize(visit),
            ActorUserId = actorUserId,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkMissedAsync(Guid visitId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var visit = await context.Visits.SingleAsync(v => v.Id == visitId, cancellationToken);

        if (visit.Status != VisitStatus.Scheduled)
            throw new InvalidOperationException($"Only scheduled visits can be marked as missed. Current status: {visit.Status}.");

        visit.Status = VisitStatus.Missed;
        visit.UpdatedAtUtc = DateTimeOffset.UtcNow;
        visit.LastModifiedBy = actorUserId;

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(Visit),
            EntityId = visit.Id,
            Operation = OperationType.Update,
            PayloadJson = JsonSerializer.Serialize(visit),
            ActorUserId = actorUserId,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
