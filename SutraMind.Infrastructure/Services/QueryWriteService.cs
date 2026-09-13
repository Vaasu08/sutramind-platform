using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Domain.Rules;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class QueryWriteService(LocalDbContext context) : IQueryWriteService
{
    public async Task RaiseAsync(RaiseQueryRequest request, Guid raisedBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Query message is required.");

        var query = new DataQuery
        {
            StudyId = request.StudyId,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            FieldName = request.FieldName,
            Message = request.Message.Trim(),
            Status = QueryStatus.Open,
            RaisedBy = raisedBy
        };

        context.Queries.Add(query);

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(DataQuery),
            EntityId = query.Id,
            Operation = OperationType.Create,
            PayloadJson = JsonSerializer.Serialize(query),
            ActorUserId = raisedBy,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AnswerAsync(Guid queryId, AnswerQueryRequest request, Guid answeredBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Answer))
            throw new ArgumentException("Answer text is required.");

        var query = await context.Queries.SingleAsync(q => q.Id == queryId, cancellationToken);

        if (!LifecycleRules.CanTransition(query.Status, QueryStatus.Answered))
            throw new InvalidOperationException($"Cannot answer a query in status '{query.Status}'.");

        query.Status = QueryStatus.Answered;
        query.Answer = request.Answer.Trim();
        query.AnsweredBy = answeredBy;
        query.AnsweredAtUtc = DateTimeOffset.UtcNow;
        query.UpdatedAtUtc = DateTimeOffset.UtcNow;
        query.LastModifiedBy = answeredBy;

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(DataQuery),
            EntityId = query.Id,
            Operation = OperationType.Update,
            PayloadJson = JsonSerializer.Serialize(query),
            ActorUserId = answeredBy,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CloseAsync(Guid queryId, CloseQueryRequest request, Guid closedBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ResolutionReason))
            throw new ArgumentException("Resolution reason is required.");

        var query = await context.Queries.SingleAsync(q => q.Id == queryId, cancellationToken);

        if (!LifecycleRules.CanTransition(query.Status, QueryStatus.Closed))
            throw new InvalidOperationException($"Cannot close a query in status '{query.Status}'.");

        query.Status = QueryStatus.Closed;
        query.ClosedBy = closedBy;
        query.ClosedAtUtc = DateTimeOffset.UtcNow;
        query.UpdatedAtUtc = DateTimeOffset.UtcNow;
        query.LastModifiedBy = closedBy;

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(DataQuery),
            EntityId = query.Id,
            Operation = OperationType.Update,
            PayloadJson = JsonSerializer.Serialize(query),
            ActorUserId = closedBy,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
