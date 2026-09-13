using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public sealed class OutboxOperation
{
    public Guid OperationId { get; init; } = Guid.NewGuid();
    public required string EntityType { get; init; }
    public required Guid EntityId { get; init; }
    public OperationType Operation { get; init; }
    public long? BaseServerVersion { get; init; }
    public required string PayloadJson { get; init; }
    public DateTimeOffset OccurredAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public Guid ActorUserId { get; init; }
    public Guid DeviceId { get; init; }
    public int AttemptCount { get; set; }
    public DateTimeOffset? NextAttemptAtUtc { get; set; }
    public OperationState State { get; set; } = OperationState.Pending;
    public string? LastErrorCode { get; set; }
    public string? LastErrorMessage { get; set; }
}
