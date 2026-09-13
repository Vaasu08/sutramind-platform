using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class EthicsWriteService(LocalDbContext context) : IEthicsWriteService
{
    public async Task UpdateStatusAsync(Guid ethicsId, EthicsStatus newStatus, DateOnly? approvalDate, DateOnly? expiryDate, string? remarks, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var ethics = await context.EthicsReviews.SingleAsync(e => e.Id == ethicsId, cancellationToken);

        ethics.Status = newStatus;
        ethics.ApprovalDate = approvalDate;
        ethics.ExpiryDate = expiryDate;
        ethics.Remarks = remarks;
        ethics.UpdatedBy = actorUserId;
        ethics.UpdatedAtUtc = DateTimeOffset.UtcNow;
        ethics.LastModifiedBy = actorUserId;

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(EthicsReview),
            EntityId = ethics.Id,
            Operation = OperationType.Update,
            PayloadJson = JsonSerializer.Serialize(ethics),
            ActorUserId = actorUserId,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
