using SutraMind.Domain.Enums;

namespace SutraMind.Application.Contracts;

public sealed record SyncPullRequest(string? Cursor, int Limit = 100);
public sealed record SyncChange(string EntityType, Guid EntityId, OperationType Operation, long? ServerVersion, string PayloadJson, DateTimeOffset OccurredAtUtc);
public sealed record SyncPullResponse(string? NextCursor, IReadOnlyList<SyncChange> Changes, DateTimeOffset ServerTimeUtc);
public sealed record SyncPushOperation(Guid OperationId, string EntityType, Guid EntityId, OperationType Operation, long? BaseServerVersion, string PayloadJson, DateTimeOffset OccurredAtUtc, Guid ActorUserId, Guid DeviceId);
public sealed record SyncPushRequest(IReadOnlyList<SyncPushOperation> Operations);
public sealed record SyncPushResult(Guid OperationId, SyncResultStatus Status, long? ServerVersion, string? ErrorCode, string? ErrorMessage, string? ServerPayloadJson);
public sealed record SyncPushResponse(IReadOnlyList<SyncPushResult> Results);
public enum SyncResultStatus { Applied, AlreadyApplied, Conflict, Rejected }
